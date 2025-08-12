using System;
using System.Net;
using System.Runtime.CompilerServices;
using WindwardRestApi.src.Api;
using WindwardRestApi.src.Model;

namespace RESTful_Client_.NET_Sample
{


    // TODO: In readme add instructions for changing which class to run
    // In Project > Properties > Application > Startup object, change the startup object to GenerateDocument, RetrieveMetrics or RetrieveTagTree depending on which functionality you want to test.


    // dotnet build RESTful-Client-.NET-Sample.csproj -p:StartupObject=RESTful_Client_.NET_Sample.GenerateDocument -t:Rebuild
    public class GenerateDocument
    {
        // Set your RESTful Engine URL here
        private static string RESTFUL_ENGINE_URL = "{YOUR_RESTFUL_ENGINE_URL}";

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

            // Pass in the License Key for the RESTful Engine (if you have one)
            // If you don't have a license key, you can leave this line blank and it will produce output with a watermark.
            client.LicenseKey = "";

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
             * Step 2: GENERATE THE REPORT
             * --------------------------------------------------------------------------------------------
             */

            Console.WriteLine("Generating report...");

            // Post our template generation request to the RESTful Engine
            Document document = await client.PostDocument(template);

            // Wait until the document generation is complete before proceeding
            while (true)
            {
                Thread.Sleep(100);
                HttpStatusCode status = await client.GetDocumentStatus(document.Guid);
                if (status == HttpStatusCode.Found)
                {
                    // The document generation is complete, we can now proceed to retrieve the generated document
                    break;
                }
                else if (status == HttpStatusCode.Accepted || status == HttpStatusCode.Created || status == HttpStatusCode.NotFound)
                {
                    // The document generation is still in progress, continue waiting
                    continue;
                }
                else
                {
                    // Potentially have an error, proceed to retrieve the document to get error details
                    Console.WriteLine("Error retrieving document. Status code: " + status);
                    break;
                }
            }

            /**
             * Step 3: RETRIEVE THE GENERATED DOCUMENT
             * --------------------------------------------------------------------------------------------
             */
            Console.WriteLine("Retrieving generated document...");

            // TODO: Test with invalid license key to make sure retriving errors works as well
            document = await client.GetDocument(document.Guid);

            // Write out the generated document to a file (ensure the file format matches the output format specified in the template)
            string outputFilePath = "../../../files/Output.pdf";
            await File.WriteAllBytesAsync(outputFilePath, document.Data);

            Console.WriteLine("Generated document saved to: " + outputFilePath);
        }
    }
}