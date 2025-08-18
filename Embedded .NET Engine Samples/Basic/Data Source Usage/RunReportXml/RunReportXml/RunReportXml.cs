using net.windward.api.csharp;
using WindwardInterfaces.net.windward.api.csharp;
using System.IO;

namespace RunReportXml
{
    class RunReportXml
    {
        static void Main(string[] args)
        {
            // Initialize the engine
            Report.Init();

            // Open template file and create output file
            FileStream template = File.OpenRead("../../../Samples/BasicXmlTemplate.docx");
            FileStream output = File.Create("../../../Samples/Output.pdf");

            // Create report process
            Report myReport = new ReportPdf(template, output);

            // Open a data object to connect to our xml file
            string url = Path.GetFullPath("../../../Samples/BasicXmlData.xml");
            string xsd = null;
            IReportDataSource data = new SaxonDataSourceImpl(string.Format("Url={0}", url), xsd);


            // Run the report process
            myReport.ProcessSetup();
            // The second parameter is "" to tell the process that our data is the default data source
            myReport.ProcessData(data, "Orders");
            myReport.ProcessComplete();

            // Close out of our template file and output
            data.Close();
            output.Close();
            template.Close();

            // Opens the finished report
            string fullPath = Path.GetFullPath("../../../Samples/Output.pdf");
            System.Diagnostics.Process.Start(fullPath);
        }
    }
}