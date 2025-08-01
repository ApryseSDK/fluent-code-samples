using net.windward.api.csharp;
using WindwardInterfaces.net.windward.api.csharp;
using System.IO;

namespace FluentCustomCallbacksTest
{
    internal class FluentCustomCallbacksTest
    {
        static void Main(string[] args)
        {
            // Initialize the engine
            Report.Init();

            // Open template file and create output file
            FileStream template = File.OpenRead("../../Samples/FluentCustomCallbacksTemplate.docx");
            FileStream output = File.Create("../../Samples/output.pdf");

            // Create report process
            Report myReport = new ReportPdf(template, output);

            // Open a data object to connect to our xml file
            string url = "http://xml.windward.net/Southwind.xml";
            string xsd = null;
            IReportDataSource data = new SaxonDataSourceImpl(string.Format("Url={0}", url), xsd);

            // Run the report process
            myReport.ProcessSetup();
            myReport.ProcessData(data, "XML");
            myReport.ProcessComplete();

            // Close out of our template file and output
            data.Close();
            output.Close();
            template.Close();

            // Opens the finished report
            string fullPath = Path.GetFullPath("../../Samples/output.pdf");
            System.Diagnostics.Process.Start(fullPath);
        }
    }
}
