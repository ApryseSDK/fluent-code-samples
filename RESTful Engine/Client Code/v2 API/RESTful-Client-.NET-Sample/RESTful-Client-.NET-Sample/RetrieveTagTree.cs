using System.Configuration;
using System.Net;
using WindwardRestApi.src.Api;
using WindwardRestApi.src.Model;

namespace RESTful_Client_.NET_Sample
{
    /// <summary>
    /// Sample code to retrieve template tag tree using the Fluent v2 RESTful Engine Client in .NET.
    /// </summary>
    public class RetrieveTagTree
    {
        // Set your RESTful Engine URL in the App.config file under the key "restful-engine-url"
        private static string RESTFUL_ENGINE_URL = ConfigurationManager.AppSettings["restful-engine-url"] ?? "";

        // Initialize the WindwardClient with the RESTful Engine URL
        private static WindwardClient client;

        public static async Task Main(string[] args)
        {
            /**
             * Step 1: INITIALIZE THE RESTFUL CLIENT AND SET UP THE TEMPLATE AND DATA SOURCE
             * --------------------------------------------------------------------------------------------
             */
            Console.WriteLine("Initializing Windward RESTful Client...");

            // Initialize the RESTful Client
            client = new WindwardClient(new Uri(RESTFUL_ENGINE_URL));

            // Pass in the License Key for the RESTful Engine (if you have one) in the App.config file under the key "license"
            // If you don't have a license key, you can leave it blank and it will produce output with a watermark.
            client.LicenseKey = ConfigurationManager.AppSettings["license"] ?? "";

            // Display the current engine version of your Fluent RESTful Engine
            VersionInfo version = await client.GetVersion();
            Console.WriteLine("Fluent RESTful Engine Version: " + version.EngineVersion);

            // Get our template and data source as byte arrays to pass to the RESTful Engine Client
            byte[] templateBytes = await File.ReadAllBytesAsync("../../../files/InvestmentFactSheet.docx");
            byte[] dataSourceBytes = await File.ReadAllBytesAsync("../../../files/InvestmentFactSheet.xml");

            // Create our template object that will output to PDF format
            Template template = new Template(Template.OutputFormatEnum.Pdf, templateBytes, Template.FormatEnum.Docx);

            // Create our data source object for our template
            Xml_10DataSource dataSource = new Xml_10DataSource("InvestmentFactSheet", dataSourceBytes, null);

            // Add the data source to the template
            template.Datasources = new List<DataSource> { dataSource };

            /**
             * Step 2: REQUEST TEMPLATE TAG TREE
             * --------------------------------------------------------------------------------------------
             */

            Console.WriteLine("Requesting Tag Tree...");

            // Post our template metrics request to the RESTful Engine
            TagTree tagTree = await client.PostTagTree(template);

            // Wait until retrieving the template tag tree request is finished before proceeding
            while (true)
            {
                Thread.Sleep(100);
                HttpStatusCode status = await client.GetTagTreeStatus(tagTree.Guid);
                if (status == HttpStatusCode.Found)
                {
                    // The tag tree retrieval is complete, we can now proceed
                    break;
                }
                else if (status == HttpStatusCode.Accepted || status == HttpStatusCode.Created || status == HttpStatusCode.NotFound)
                {
                    // The tag tree retrieval is still in progress, continue waiting
                    continue;
                }
                else
                {
                    // Potentially have an error, proceed to retrieve the tag tree to get error details
                    Console.WriteLine("Error retrieving tag tree. Status code: " + status);
                    break;
                }
            }

            /**
             * Step 3: RETRIEVE THE TAG TREE
             * --------------------------------------------------------------------------------------------
             */
            Console.WriteLine("Retrieving Tag Tree...");
            tagTree = await client.GetTagTree(tagTree.Guid);

            // Write out the tag tree XML to a file
            string outputFilePath = "../../../files/TagTree.xml";
            await File.WriteAllBytesAsync(outputFilePath, tagTree.Xml);

            Console.WriteLine("Template Tag Tree saved to: " + outputFilePath);
        }
    }
}
