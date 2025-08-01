import net.windward.datasource.DataSourceProvider;
import net.windward.datasource.xml.SaxonDataSource;
import net.windward.xmlreport.ProcessPdf;
import net.windward.xmlreport.ProcessReport;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;

/**
 * A simple test class generating a document from a template that references our custom function
 */
public class FluentCustomFunctionsTest {
    public static void main(String[] args) {
        try {
            // To generate a report, first we need a ProcessReport object.  For now, we're using the
            // pdf format to output.
            File fileReport = new File("Samples/output.pdf");
            FileInputStream template = new FileInputStream("Samples/FluentCustomFunctionTemplate.docx");
            FileOutputStream reportStream = new FileOutputStream(fileReport);
            ProcessReport report = new ProcessPdf(template, reportStream);

            // Preparation...
            report.processSetup();

            // Set up the datasource. The parameters are connector package, url, username, password.
            // For each type of datasource, the connector package is different
            DataSourceProvider datasource = new SaxonDataSource("http://xml.windward.net/Southwind.xml", null);

            // Finally, send it to Fluent for processing.  The second parameter is the name of the
            // datasource.  This should match the name used in your template.
            report.processData(datasource, "XML");

            // And... DONE!
            report.processComplete();
            template.close();
            reportStream.close();
            System.out.println("Finished generating output: " + fileReport.getAbsolutePath());
        } catch (Exception e) {
            // Uh oh, just in case
            e.printStackTrace();
        }
    }
}
