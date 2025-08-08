import java.awt.*;
import java.io.*;

import net.windward.datasource.DataSourceProvider;
import net.windward.datasource.jdbc.JdbcDataSource;
import net.windward.xmlreport.ProcessPdf;
import net.windward.xmlreport.ProcessReport;
import net.windward.xmlreport.ProcessReportAPI;
import net.windward.env.SystemWrapper;

/*
 * A sample usage of the Fluent Java Engine. This is the first building block to a great report!
 *
 * This sample takes in a sample sql datasource and a docx template file--in this case, template.docx
 * in the data directory--and produces the report, stored as report.pdf in the out directory.
 *
 * Take a look, the results are amazing!
 */
public class BasicDb2 {
    public static void main(String[] args) {
        try {
            try {
                Class.forName("com.ibm.db2.jcc.DB2Driver").newInstance();
            } catch (ClassNotFoundException e) {
                throw new ClassNotFoundException("Please add the DB2 JDBC connector to your classpath. Details at http://rpt.me/DB2JDBC", e);
            }

            // To generate a report, first we need a ProcessReport object.  For now, we're using the
            // pdf format to output.
            File fileReport = new File("out/report.pdf");
            fileReport.getParentFile().mkdirs();

            FileInputStream template = new FileInputStream("YOUR_TEMPLATE_PATH");
            FileOutputStream reportStream = new FileOutputStream(fileReport);
            ProcessReport report = new ProcessPdf(template, reportStream);

            // Preparation...
            report.processSetup();

            // Set up the datasource. The parameters are connector package, url, username, password.
            // For each type of datasource, the connector package is different
            DataSourceProvider datasource = new JdbcDataSource("com.ibm.db2.jcc.DB2Driver",
                    "YOUR_DATASOURCE_CONNECTION_STRING", "YOUR_DATASOURCE_USERNAME", "YOUR_DATASOURCE_PASSWORD");

            // Finally, send it to Fluent for processing.  The second parameter is the name of the
            // datasource.  This should match the name used in your template.
            report.processData(datasource, "DB2");

            // And... DONE!
            report.processComplete();
            template.close();
            reportStream.close();
            if (Desktop.isDesktopSupported()) {
                Desktop desktop = Desktop.getDesktop();
                System.out.println("Launching report " + fileReport.getAbsolutePath());
                if (fileReport.exists() && fileReport.isFile()) {
                    desktop.open(fileReport);
                }
            }

        } catch (Exception e) {
            // Uh oh, just in case
            e.printStackTrace();
        }
    }
}