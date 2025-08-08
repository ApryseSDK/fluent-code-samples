import net.windward.datasource.DataSourceProvider;
import net.windward.datasource.salesforce.SalesForceDataSource;
import net.windward.xmlreport.ProcessPdf;
import net.windward.xmlreport.ProcessReport;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.util.HashMap;
import java.util.Map;

public class BasicSalesforce {

    /**
     * Normally you will copy the code in this method to your application code.
     * @param args unused
     */

    public static void main(String[] args) throws Exception {

        File fileReport = new File("out/report.pdf");
        fileReport.getParentFile().mkdirs();

        FileInputStream template = new FileInputStream("YOUR_TEMPLATE_FILE_PATH");
        FileOutputStream reportStream = new FileOutputStream(fileReport);
        ProcessReport report = new ProcessPdf(template, reportStream);

        // Preparation...
        report.processSetup();

        // Place all variables in this map. We assign this map to all datasources.
        Map<String, Object> mapVariables = new HashMap<String, Object>();
        // add variables here using: mapVariables.put("key", value);

        Map<String, DataSourceProvider> dataSources = new HashMap<String, DataSourceProvider>();

        DataSourceProvider sfdemo = new SalesForceDataSource("YOUR_USERNAME", "YOUR_PASSWORD", "YOUR_SECURITY_TOKEN", true);
        sfdemo.setParameters(mapVariables);
        dataSources.put("YOUR_DATA_SOURCE_NAME", sfdemo);

        // Insert all data into the report.
        report.processData(dataSources);

        // You should place the following in a finally block (we did not to keep this clear).
        sfdemo.close();

        // Write the generated report to the PDF file.
        report.processComplete();

        //ensure everything is written out to the stream
        template.close();

        reportStream.close();

    }

}