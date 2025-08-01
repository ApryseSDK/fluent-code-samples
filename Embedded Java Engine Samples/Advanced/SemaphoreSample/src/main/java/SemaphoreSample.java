import net.windward.xmlreport.ProcessReport;

import java.io.File;
import java.nio.file.Files;
import java.nio.file.Paths;

/**
 * Sample application to show how to limits calls to Fluent to the license thread limit.
 */
public class SemaphoreSample {

    /**
     * Run the example.
     * @param args Run the example.
     */
    public static void main(String[] args) throws Exception {

        // initialize the engine
        ProcessReport.init();

        String path = new File("files").getAbsolutePath();
        String template = new File(path, "Windward Trucking 2 - Template.docx").getAbsolutePath();
        String xmlData = new File(path, "Windward Trucking 2 - Data.xml").getAbsolutePath();

        Files.createDirectories(Paths.get("out"));
        File reportFolder = new File("out");

        // set up the reports I want to run
        GenerateDocument[] myReports = new GenerateDocument[10];
        for (int ind = 0; ind < myReports.length; ind++)
            myReports[ind] = new GenerateDocument(template, xmlData, new File(reportFolder, "Report_" + ind + ".docx").getAbsolutePath());

        // create a thread for each request
        Thread[] myThreads = new Thread[myReports.length];
        for (int ind = 0; ind < myReports.length; ind++ )
            myThreads[ind] = new Thread(myReports[ind]);

        // start the threads
        for (int ind = 0; ind < myThreads.length; ind++ )
            myThreads[ind].start();

        // wait for them to end
        for (int ind = 0; ind < myThreads.length; ind++ )
            myThreads[ind].join();

        System.out.println("all threads completed");
    }
}
