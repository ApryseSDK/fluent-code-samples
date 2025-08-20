using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using log4net;
using SampleRESTfulS3Plugin.Models;
using SampleRESTfulS3Plugin.S3Storage;
using WindwardModels;
using WindwardRepository;
using Document = WindwardModels.Document;
using Metrics = WindwardModels.Metrics;

namespace SampleRESTfulS3Plugin
{
	public class S3Repository : IRepository
	{
		private string bucketName = "";
		private string awsAccessKey = "";
		private string awsSecretKey = "";

		BasicAWSCredentials aWSCredentials;
		S3StorageManager storageManager;
		private IAmazonS3 s3Client;
		TransferUtility transferUtility;

		private readonly bool runningOnIIS;

		/// <summary>
		/// Used by the main worker to wake it up.
		/// </summary>
		private AutoResetEvent eventSignal;

		/// <summary>
		/// true when we're shutting down.
		/// </summary>
		private bool shutDown;

		/// <summary>
		/// Delete any jobs older than this.
		/// </summary>
		private readonly TimeSpan timeSpanDeleteOldJobs;

		/// <summary>
		/// How often we check for old jobs.
		/// </summary>
		private readonly TimeSpan timeSpanCheckOldJobs;

		private DateTime datetimeLastCheckOldJobs;

		/// <summary>
		/// Requested jobs, need to write to disk.
		/// </summary>
		private readonly ConcurrentQueue<Tuple<Template, RepositoryStatus.REQUEST_TYPE>> queueRequests;

		/// <summary>
		/// Requested job deletes. Need to delete the files.
		/// </summary>
		private ConcurrentQueue<string> queueDeletes;

		private static readonly ILog Log = LogManager.GetLogger(typeof(S3Repository));



        /// <summary>
        /// Constructor for the S3Repository.
        /// Initializes AWS credentials, S3 client, transfer utility, and storage manager. 
        /// Also sets up job deletion schedules and background workers.
        /// </summary>
        public S3Repository()
		{
			aWSCredentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
			s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, RegionEndpoint.USEast1);
			transferUtility = new TransferUtility(s3Client);
			storageManager = new S3StorageManager(aWSCredentials, transferUtility, s3Client, bucketName);

			if (!AmazonS3Util.DoesS3BucketExistV2(s3Client, bucketName))
			{
				Log.Debug("Bucket doesnt exist, creating bucket with name: " + bucketName);
				PutBucketRequest putBucketRequest = new PutBucketRequest()
				{
					BucketName = bucketName,
					UseClientRegion = true
				};
				Log.Debug("Created bucket with name: " + bucketName);
			}
			else
			{
				Log.Debug("Found bucket with name: " + bucketName + " ... proceeding");
			}

			string num = ConfigurationManager.AppSettings["hours-delete-jobs"] ?? "";
			if (!int.TryParse(num, out int hours))
				hours = 24;
			else
				hours = Math.Max(1, hours);
			timeSpanDeleteOldJobs = TimeSpan.FromHours(hours);

			// check every 24th of timeSpanDeleteOldJobs. So if delete 24+ hours old, check every hour
			timeSpanCheckOldJobs = TimeSpan.FromMinutes(timeSpanDeleteOldJobs.TotalMinutes / 24);

			// we give it the timespan until the first check - to first get anything it grabs at startup.
			datetimeLastCheckOldJobs = DateTime.Now;

			queueRequests = new ConcurrentQueue<Tuple<Template, RepositoryStatus.REQUEST_TYPE>>();
			queueDeletes = new ConcurrentQueue<string>();

			if (Log.IsInfoEnabled)
			{
				Log.Info($"starting FileSystemRepository {(runningOnIIS ? "on IIS" : "on .Net")}, Delete jobs older than {timeSpanDeleteOldJobs}");
			}

			eventSignal = new AutoResetEvent(true);

			// This thread manages all the background threads. It sleeps on an event and when awoken, fires off anything it can.
			// This is used so web requests that call signal aren't delayed as background tasks might be started.
			runningOnIIS = true;
			if (runningOnIIS)
				HostingEnvironment.QueueBackgroundWorkItem(ct => ManageOldRequests(ct));
			else
			{
				var tokenSource = new CancellationTokenSource();
				var token = tokenSource.Token;
				Task.Run(() => ManageOldRequests(token), token);
			}
		}

		private IJobHandler JobHandler { get; set; }

		/// <summary>
		/// Give it the repository at startup. Only call this once.
		/// </summary>
		/// <param name="handler">The job handler that will process requests in this repository.</param>
		public void SetJobHandler(IJobHandler handler)
		{
			if (Log.IsDebugEnabled)
				Log.Debug($"SetJobHandler({handler})");
			JobHandler = handler;
		}

		public void ShutDown()
		{
			Log.Debug("[S3RepoPlugin] AzureRepositoryPlugin.ShutDown() started...");
			shutDown = true;
			//_producer.Stop();

			Log.Debug("[S3RepoPlugin] AzureRepositoryPlugin bus stopped");

			// Need to set all generating requests in azure storage back to pending
			var task = Task.Run(async () => await ShutDownAsync());
			task.Wait();
		}

		private async Task<bool> ShutDownAsync()
		{
			bool success = await storageManager.RevertGeneratingJobsToPending();
			if (success)
				Log.Debug("[S3RepoPlugin] All generating jobs reverted to pending");
			return success;
		}

        /// <summary>
        /// Creates a new job request and returns its unique identifier.
        /// </summary>
        /// <param name="template">The template for the request.</param>
        /// <param name="requestType">The type of request.</param>
        /// <returns>The GUID of the created request.</returns>
        public string CreateRequest(Template template, RepositoryStatus.REQUEST_TYPE requestType)
		{
			try
			{
				var task = Task.Run(async () => await CreateRequestAsync(template, requestType));
				task.Wait();
				string guid = task.Result;
				return guid;
			}
			catch (Exception ex)
			{
				Log.Error($"CreateRequest({template.Guid}, {requestType})", ex);
				throw;
			}
		}

        /// <summary>
        /// Creates a new job request asynchronously.
        /// </summary>
        /// <param name="template">The template for the request.</param>
        /// <param name="requestType">The type of request.</param>
        /// <returns>A task that returns the GUID of the created request.</returns>
        public async Task<string> CreateRequestAsync(Template template, RepositoryStatus.REQUEST_TYPE requestType)
		{
			template.Guid = Guid.NewGuid().ToString();

			JobRequestData jobData = new JobRequestData
			{
				Template = template,
				RequestType = requestType,
				CreationDate = DateTime.UtcNow
			};

			Log.Info($"[S3RepoPlugin] Created request {jobData.Template.Guid}");

			bool success = await storageManager.AddRequest(jobData);

			if (!success)
			{
				Log.Error($"Failed to add job request [{jobData.Template.Guid}] to storage");
				return null;
			}

			Log.Info($"[S3RepoPlugin] Added job request [{jobData.Template.Guid}] to storage");

			JobHandler?.Signal();

			return template.Guid;
		}

        /// <summary>
        /// Takes the next request from the repository queue.
        /// </summary>
        /// <returns>The next repository request, or null if none exist.</returns>
        public RepositoryRequest TakeRequest()
		{
			Log.Info("[S3RepoPlugin] Take request called");
			try
			{
				var task = Task.Run(async () => await TakeRequestAsync());
				task.Wait();
				RepositoryRequest ret = task.Result;
				Log.Info($"[S3RepoPlugin] Take request returned {ret}");

				return ret;
			}
			catch (Exception e)
			{
				Log.Error($"[S3RepoPlugin] Exception in TakeRequest: {e.Message}");
				return null;
			}
		}

        /// <summary>
        /// Takes the next request from the repository queue asynchronously.
        /// </summary>
        /// <returns>A task that returns the next repository request, or null if none exist.</returns>
        private async Task<RepositoryRequest> TakeRequestAsync()
		{
			JobRequestData job = await storageManager.GetOldestPendingJobAndGenerate();

			if (job != null)
				Log.Info($"[S3RepoPlugin] Took reqest {job.Template.Guid}");
			else
				Log.Info($"[S3RepoPlugin] Took request NULL");

			if (job == null)
				return null;

			return new RepositoryRequest(job.Template, job.RequestType);
		}


        /// <summary>
        /// Saves a generated report to storage.
        /// </summary>
        /// <param name="template">The associated template.</param>
        /// <param name="document">The document to save.</param>
        public void SaveReport(Template template, Document document)
		{
			try
			{
				var task = Task.Run(async () => await SaveReportAsync(template, document));
				task.Wait();
				CompleteJob(template);
			}
			catch (Exception e)
			{
				Log.Error($"[S3RepoPlugin] Exception in SaveReport: {e.Message}");
			}
		}

        /// <summary>
        /// Saves a generated report asynchronously.
        /// </summary>
        /// <param name="template">The associated template.</param>
        /// <param name="document">The document to save.</param>
        /// <returns>A task representing the save operation.</returns>
        private async Task SaveReportAsync(Template template, Document document)
		{
			var result = await storageManager.CompleteRequest(template.Guid, document);
			if (result)
				Log.Debug($"[S3RepoPlugin] Successfully saved document {template.Guid}");
			else
				Log.Error($"Failed to save document {template.Guid}");
		}


        /// <summary>
        /// Saves metrics associated with a request.
        /// </summary>
        /// <param name="template">The associated template.</param>
        /// <param name="metrics">The metrics to save.</param>
        public void SaveMetrics(Template template, Metrics metrics)
		{
			try
			{
				var task = Task.Run(async () => await SaveMetricsAsync(template, metrics));
				task.Wait();
				CompleteJob(template);
			}
			catch (Exception e)
			{
				Log.Error($"[S3RepoPlugin] Exception in SaveMetrics: {e.Message}");
			}
		}

        /// <summary>
        /// Saves metrics asynchronously.
        /// </summary>
        /// <param name="template">The associated template.</param>
        /// <param name="metrics">The metrics to save.</param>
        /// <returns>A task representing the save operation.</returns>
        private async Task SaveMetricsAsync(Template template, Metrics metrics)
		{
			var result = await storageManager.CompleteRequest(template.Guid, metrics);
			if (result)
				Log.Debug($"[S3RepoPlugin] Successfully saved metrics {template.Guid}");
			else
				Log.Error($"Failed to save metrics {template.Guid}");
		}

        /// <summary>
        /// Saves a tag tree associated with a request.
        /// </summary>
        /// <param name="template">The associated template.</param>
        /// <param name="tree">The tag tree to save.</param>
        public void SaveTagTree(Template template, TagTree tree)
		{
			try
			{
				var task = Task.Run(async () => await SaveTagTreeAsync(template, tree));
				task.Wait();
				CompleteJob(template);
			}
			catch (Exception e)
			{
				Log.Error($"[S3RepoPlugin] Exception in SaveTagTree: {e.Message}");
			}
		}

        /// <summary>
        /// Saves a tag tree asynchronously.
        /// </summary>
        /// <param name="template">The associated template.</param>
        /// <param name="tree">The tag tree to save.</param>
        /// <returns>A task representing the save operation.</returns>
        private async Task SaveTagTreeAsync(Template template, TagTree tree)
		{
			var result = await storageManager.CompleteRequest(template.Guid, tree);
			if (result)
				Log.Debug($"[S3RepoPlugin] Successfully saved tag tree {template.Guid}");
			else
				Log.Error($"Failed to save tag tree {template.Guid}");
		}

        /// <summary>
        /// Completes a job by calling back to its callback URL.
        /// </summary>
        /// <param name="template">The template whose job is being completed.</param>
        private void CompleteJob(Template template)
		{
			if (shutDown || string.IsNullOrEmpty(template.Callback))
				return;

			string url = template.Callback.Replace("{guid}", template.Guid);
			try
			{
				using (HttpClient client = new HttpClient())
				using (HttpResponseMessage response = client.PostAsync(url, null).Result)
					if (response.StatusCode != HttpStatusCode.OK && Log.IsInfoEnabled)
						Log.Info($"[S3RepoPlugin] Callback to {url} returned status code {response.StatusCode}");
			}
			catch (Exception ex)
			{
				Log.Warn($"Callback for job {template.Guid} to url {template.Callback} threw exception {ex.Message}", ex);
				// silently swallow the exception - this is a background thread.
			}
		}

        /// <summary>
        /// Saves an error associated with a request.
        /// </summary>
        /// <param name="template">The associated template.</param>
        /// <param name="error">The error details.</param>
        public void SaveError(Template template, ServiceError error)
		{
			try
			{
				var task = Task.Run(async () => await SaveErrorAsync(template, error));
				task.Wait();
				CompleteJob(template);
			}
			catch (Exception e)
			{
				Log.Error($"[S3RepoPlugin] Exception in SaveError: {e.Message}");
			}
		}

        /// <summary>
        /// Saves an error asynchronously.
        /// </summary>
        /// <param name="template">The associated template.</param>
        /// <param name="error">The error details.</param>
        /// <returns>A task representing the save operation.</returns>
        private async Task SaveErrorAsync(Template template, ServiceError error)
		{
			var result = await storageManager.UpdateRequest(template.Guid, RepositoryStatus.JOB_STATUS.Error);
			if (!result)
				Log.Error($"Failed to save error status {template.Guid}");

			result = await storageManager.CompleteRequest(template.Guid, error);
			if (result)
				Log.Debug($"[S3RepoPlugin] Successfully saved error {template.Guid}");
			else
				Log.Error($"Failed to save error {template.Guid}");
		}

        /// <summary>
        /// Gets the status of a report.
        /// </summary>
        /// <param name="guid">The unique request identifier.</param>
        /// <returns>The request status, or null if an error occurs.</returns>
        public RequestStatus GetReportStatus(string guid)
        {
            var task = Task.Run(async () => await GetReportStatusAsync(guid));
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Gets the status of a report asynchronously.
        /// </summary>
        /// <param name="guid">The unique request identifier.</param>
        /// <returns>A task returning the request status, or null if an error occurs.</returns>
        public async Task<RequestStatus> GetReportStatusAsync(string guid)
		{
			try
			{
				JobInfoEntity result = await storageManager.GetRequestInfo(guid);
				return new RequestStatus((RepositoryStatus.JOB_STATUS)result.Status, (RepositoryStatus.REQUEST_TYPE)result.Type);
			}
			catch (Exception e)
			{
				Log.Error($"[S3RepoPlugin] Exception in GetReportStatus: {e.Message}");
				return null;
			}
		}

        /// <summary>
        /// Retrieves a generated report by GUID.
        /// </summary>
        /// <param name="guid">The unique request identifier.</param>
        /// <returns>The document, or null if not found.</returns>
        public Document GetReport(string guid)
		{
			try
			{
				Task<Document> task = Task.Run<Document>(async () => await GetReportAsync(guid));
				task.Wait();
				Document res = task.Result;
				return res;
			}
			catch (Exception e)
			{
				Log.Error($"[S3RepoPlugin] Exception in GetReport: {e.Message}");
				return null;
			}
		}

        /// <summary>
        /// Retrieves a generated report asynchronously.
        /// </summary>
        /// <param name="guid">The unique request identifier.</param>
        /// <returns>A task returning the document, or null if not found.</returns>
        private async Task<Document> GetReportAsync(string guid)
		{
			var result = await storageManager.GetGeneratedReport(guid);
			return result;
		}


        /// <summary>
        /// Retrieves metadata for a report by GUID.
        /// </summary>
        /// <param name="guid">The unique request identifier.</param>
        /// <returns>The document metadata, or null if not found.</returns>
        public DocumentMeta GetReportMeta(string guid)
        {
            try
            {
                var task = Task.Run<DocumentMeta>(async () => await GetReportMetaAsync(guid));
                task.Wait();
                DocumentMeta res = task.Result;
                return res;
            }
            catch (Exception e)
            {
                Log.Error($"[S3RepoPlugin] Exception in GetReportMeta: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Retrieves metadata for a report asynchronously.
        /// </summary>
        /// <param name="guid">The unique request identifier.</param>
        /// <returns>A task returning the document metadata.</returns>
        public async Task<DocumentMeta> GetReportMetaAsync(string guid)
        {
            Document doc = await storageManager.GetGeneratedReport(guid);
            DocumentMeta ret = SetReportMeta(doc);
            return ret;
        }

        /// <summary>
        /// Retrieves metrics by GUID.
        /// </summary>
        /// <param name="guid">The unique request identifier.</param>
        /// <returns>The metrics, or null if not found.</returns>
        public Metrics GetMetrics(string guid)
		{
			try
			{

			}
			catch (Exception e)
			{
				Log.Error($"[S3RepoPlugin] Exception in GetMetrics: {e.Message}");
			}
			var task = Task.Run<Metrics>(async () => await GetMetricsAsync(guid));
			task.Wait();
			Metrics res = task.Result;
			return res;
		}

        /// <summary>
        /// Retrieves metrics asynchronously.
        /// </summary>
        /// <param name="guid">The unique request identifier.</param>
        /// <returns>A task returning the metrics, or null if not found.</returns>
        private async Task<Metrics> GetMetricsAsync(string guid)
		{
			var result = await storageManager.GetMetrics(guid);
			return result;
		}


        /// <summary>
        /// Retrieves error details by GUID.
        /// </summary>
        /// <param name="guid">The unique request identifier.</param>
        /// <returns>The error, or null if not found.</returns>
        public ServiceError GetError(string guid)
		{
			try
			{
				var task = Task.Run<ServiceError>(async () => await GetErrorAsync(guid));
				task.Wait();
				ServiceError res = task.Result;
				return res;
			}
			catch (Exception e)
			{
				Log.Error($"[S3RepoPlugin] Exception in GetError: {e.Message}");
				return null;
			}
		}

        /// <summary>
        /// Retrieves error details asynchronously.
        /// </summary>
        /// <param name="guid">The unique request identifier.</param>
        /// <returns>A task returning the error, or null if not found.</returns>
        private async Task<ServiceError> GetErrorAsync(string guid)
		{
			var result = await storageManager.GetError(guid);
			return result;
		}

        /// Retrieves a tag tree by GUID.
        /// </summary>
        /// <param name="guid">The unique request identifier.</param>
        /// <returns>The tag tree, or null if not found.</returns>
        public TagTree GetTagTree(string guid)
		{
			try
			{
				var task = Task.Run<TagTree>(async () => await GetTagTreeAsync(guid));
				task.Wait();
				TagTree res = task.Result;
				return res;
			}
			catch (Exception e)
			{
				Log.Error($"[S3RepoPlugin] Exception in GetTagTree: {e.Message}");
				return null;
			}
		}

        /// <summary>
        /// Retrieves a tag tree asynchronously.
        /// </summary>
        /// <param name="guid">The unique request identifier.</param>
        /// <returns>A task returning the tag tree, or null if not found.</returns>
        private async Task<TagTree> GetTagTreeAsync(string guid)
		{
			var result = await storageManager.GetTagTree(guid);
			return result;
		}

        /// <summary>
        /// Deletes a report by GUID.
        /// </summary>
        /// <param name="guid">The unique request identifier.</param>
        public void DeleteReport(string guid)
		{
			try
			{
				var task = Task.Run(async () => await DeleteReportAsync(guid));
				task.Wait();
			}
			catch (Exception e)
			{
				Log.Error($"[S3RepoPlugin] Excepition in DeleteReport {e.Message}");
			}
		}

        /// <summary>
        /// Deletes a report asynchronously by GUID.
        /// </summary>
        /// <param name="guid">The unique request identifier.</param>
        /// <returns>A task returning true if the report was deleted successfully.</returns>
        private async Task<bool> DeleteReportAsync(string guid)
		{
			bool result = await storageManager.DeleteRequest(guid);
			return result;
		}

        /// <summary>
        /// Background worker to manage old requests and delete expired jobs.
        /// </summary>
        /// <param name="ct">Cancellation token to stop execution.</param>
        private void ManageOldRequests(CancellationToken ct)
		{
			while ((!shutDown) && (!ct.IsCancellationRequested))
			{
				if (datetimeLastCheckOldJobs + timeSpanCheckOldJobs < DateTime.Now)
				{
                    DeleteOldJobs(DateTime.Now - timeSpanDeleteOldJobs);
                    datetimeLastCheckOldJobs = DateTime.Now;
				}

				// wait until needed again, or cancelled, or time to check for jobs.
				WaitHandle.WaitAny(new WaitHandle[] { eventSignal, ct.WaitHandle }, timeSpanCheckOldJobs, false);
			}
			if (Log.IsDebugEnabled)
				Log.Debug("FileSystemRepository management worker stopped");
		}

        /// <summary>
        /// Deletes old jobs older than the given cutoff date.
        /// </summary>
        /// <param name="cutoff">The cutoff datetime.</param>
        private void DeleteOldJobs(DateTime cutoff)
		{
			try
			{
				var task = Task.Run(async () => await DeleteOldJobsAsync(cutoff));
				task.Wait();
			}
			catch (Exception e)
			{
				Log.Error($"[S3RepoPlugin] Exception in DeleteOldJobs: {e.Message}");
			}
		}

        /// Deletes old jobs asynchronously.
        /// </summary>
        /// <param name="cutoff">The cutoff datetime.</param>
        /// <returns>A task representing the deletion operation.</returns>
        private async Task DeleteOldJobsAsync(DateTime cutoff)
		{
			var result = await storageManager.DeleteOldRequests(cutoff);
		}



        /// <summary>
        /// Reads a data contract object from a file in S3.
        /// </summary>
        /// <param name="filename">The file key in S3.</param>
        /// <param name="type">The type of object to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        internal object ReadDataContractFromFile(string filename, Type type)
		{
			GetObjectRequest getObjRequest = new GetObjectRequest();
			MemoryStream memoryStream = new MemoryStream();
			getObjRequest.BucketName = bucketName;
			getObjRequest.Key = filename;
			try
			{
				using (GetObjectResponse getObjRespone = s3Client.GetObject(getObjRequest))
				using (Stream responseStream = getObjRespone.ResponseStream)
				{

					XmlReaderSettings rs = new XmlReaderSettings
					{
						ConformanceLevel = ConformanceLevel.Fragment,
					};
					XmlReader r = XmlReader.Create(responseStream, rs);
					return new DataContractSerializer(type).ReadObject(r);

				}
			}
			catch (Exception ex)
			{
				Log.Error("Exception thrown in ReadDataContractFromFile: " + ex);
				throw;
			}
		}

        /// <summary>
        /// Writes a data contract object to a file in S3.
        /// </summary>
        /// <param name="data">The object to serialize and store.</param>
        /// <param name="filename">The S3 key to store the object under.</param>
        internal void WriteDataContractToFile(object data, string filename)
		{
			try
			{
                using (MemoryStream stream = new MemoryStream())
                {
                    using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
                    {
                        DataContractSerializer dcs = new DataContractSerializer(data.GetType());
                        writer.WriteStartDocument();
                        dcs.WriteObject(writer, data);
                    }
					var uploadRequest = new TransferUtilityUploadRequest();
					uploadRequest.InputStream = stream;
					uploadRequest.Key = filename;
					uploadRequest.BucketName = bucketName;
					transferUtility.Upload(uploadRequest);
				}
            }
			catch (Exception ex)
			{
				Log.Error("Error in WriteDataContractToFile, "+ex);
			}

		}

        /// <summary>
        /// Writes raw byte data to a file in S3.
        /// </summary>
        /// <param name="data">The byte array to write.</param>
        /// <param name="fileName">The S3 key to store the data under.</param>
        internal void WriteDataToFile(byte[] data, string fileName)
		{
			Stream stream = new MemoryStream(data);
			transferUtility.Upload(stream, bucketName, fileName);
		}

        /// <summary>
        /// Creates metadata for a generated document.
        /// </summary>
        /// <param name="genDoc">The generated document.</param>
        /// <returns>A document metadata object containing summary details.</returns>
        internal DocumentMeta SetReportMeta(Document genDoc)
		{
			DocumentMeta largeDoc = new DocumentMeta();
			largeDoc.Guid = genDoc.Guid;
			largeDoc.NumberOfPages = genDoc.NumberOfPages;
			largeDoc.ImportInfo = genDoc.ImportInfo;
			largeDoc.Tag = genDoc.Tag;
			largeDoc.Errors = genDoc.Errors;

			if (genDoc.Pages == null)
			{
				Uri url = HttpContext.Current.Request.Url; ;
				string tempUri = url.AbsoluteUri.ToString();
				tempUri = tempUri.Substring(0, tempUri.Length - 4);
				largeDoc.Uri = tempUri + "file";
			}

			return largeDoc;
		}

        /// <summary>
        /// Saves a performance object for a document.
        /// </summary>
        /// <param name="data">The performance data.</param>
        /// <param name="guid">The document GUID.</param>
        public void SaveDocumentPerformanceObject(DocumentPerformance data, string guid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets performance data for a document.
        /// </summary>
        /// <param name="guid">The document GUID.</param>
        /// <returns>The document performance data.</returns>
        public DocumentPerformance GetDocumentPerformance(string guid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves a cached template.
        /// </summary>
        /// <param name="template">The cached template to save.</param>
        public void SaveTemplate(CachedTemplate template)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Retrieves a cached template by ID.
        /// </summary>
        /// <param name="templateID">The template identifier.</param>
        /// <returns>The cached template.</returns>
        public CachedTemplate GetTemplate(string templateID)
        {
            throw new NotImplementedException();
        }

        public void DeleteTemplate(string templateID)
        {
            throw new NotImplementedException();
        }
    }
}
