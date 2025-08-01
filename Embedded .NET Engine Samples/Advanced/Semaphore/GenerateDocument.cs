using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Threading;
using WindwardInterfaces.net.windward.api.csharp;
using net.windward.api.csharp;

namespace SemaphoreSample
{
	/// <summary>
	/// Demonstrates how to use a semaphore to limit N threads calling Fluent at once.
	/// </summary>
	public class GenerateDocument
	{

		private readonly string templateFilename;
		private readonly string xmlDataFilename;
		private readonly string reportFilename;

		private static readonly Semaphore sem;

		/// <summary>
		/// Pulls the number of threads from the config file. If this changes you need to re-start the app.
		/// </summary>
		static GenerateDocument()
		{
			NameValueCollection appSettings = ConfigurationManager.AppSettings;
			string strNumThreads = appSettings["NumberThreads"];
			int intNumThreads = 2;
			if (strNumThreads != null)
			{
				int.TryParse(strNumThreads, out intNumThreads);
				intNumThreads = Math.Max(2, intNumThreads);
			}
			sem = new Semaphore(intNumThreads, intNumThreads);
			// if you have multiple apps running, use the following line instead
			// sem = new Semaphore(intNumThreads, intNumThreads, "MySemaphoreName");
		}

		public GenerateDocument(string templateFilename, string xmlDataFilename, string reportFilename)
		{
			this.templateFilename = templateFilename;
			this.xmlDataFilename = xmlDataFilename;
			this.reportFilename = reportFilename;
		}

        public void RunReport()
        {
            Console.Out.WriteLine(string.Format("Requesting report {0}", Path.GetFileName(reportFilename)));

            // this will not return until there is an available semaphore. When it returns, the used semaphore count is incremented by one.
            sem.WaitOne();

            try
            {
                Console.Out.WriteLine(string.Format("     processing report {0}", Path.GetFileName(reportFilename)));

                // Open template file and create output file
                using (FileStream template = File.OpenRead(templateFilename))
                using (FileStream output = File.Create(reportFilename))
                using (Report myReport = new ReportDocx(template, output))
                {
                    myReport.ProcessSetup();

                    // remove this - this is here to ensure that all threads try to run at once.
                    Thread.Sleep(10 * 1000);

                    // Read XML data into memory to avoid threading issues
                    byte[] xmlBytes = File.ReadAllBytes(xmlDataFilename);
                    using (MemoryStream xmlStream = new MemoryStream(xmlBytes))
                    {
                        IReportDataSource data = new SaxonDataSourceImpl(xmlStream);

                        myReport.ProcessData(data, "sax");
                        myReport.ProcessComplete();
                    }
                }
            }
            finally
            {
                Console.Out.WriteLine(string.Format("          report completed (releasing semaphore) {0}", Path.GetFileName(reportFilename)));

                // you must call this in a finally block so it is always called.
                // this decrements the used semaphore count by one.
                sem.Release(1);
            }
        }
    }
}
