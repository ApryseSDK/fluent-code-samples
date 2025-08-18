using System;
using System.Configuration;
using System.Threading.Tasks;


namespace Fluent
{
    class BatchProcessingApp
    {
        /// <summary>
        /// Main entry point for the program. Here we are just doing setup like grabbing the files needed to generate the document,
        /// setting the output directory, and calling the function to process the documents.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Task</returns>
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting...");

            // The location of the RESTful engine to use. Enter your RESTful engine URL in the App.config file with the key "restful-engine-url".
            string engineUrl = ConfigurationManager.AppSettings["restful-engine-url"] ?? "";

            // The license key to use for the RESTful engine. Enter your license key in the App.config file with the key "license".
            // If this is left blank, the RESTful engine will still work but it will leave a watermark on the generated documents.
            string licenseKey = ConfigurationManager.AppSettings["license"] ?? "";

            // The location of the template to use
            string templateUrl = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\net8.0\\", "\\Data\\LemonadeStandInvoice.docx");

            // The location of the JSON data file, and where we want to save the 
            // completed documents
            string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\net8.0\\", "\\Data\\LemonadeStandOrders.json");
            string saveLocation = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\net8.0\\", "\\GeneratedDocs\\");

            // Create a processor to handle document generation 
            DocumentProcessor generator = new DocumentProcessor(engineUrl, licenseKey, saveLocation);

            // Load the template into memory
            byte[] templateBytes = await System.IO.File.ReadAllBytesAsync(templateUrl);

            // Process all of the orders, generating a document from our template for each order
            await generator.ProcessOrdersJson(jsonFilePath, templateBytes);
            Console.WriteLine("Finished. Press any key to exit...");

            Console.ReadKey();
        }
    }
}