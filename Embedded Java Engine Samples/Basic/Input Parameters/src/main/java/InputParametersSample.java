/*
* Copyright (c) 2012 by Windward Studios, Inc. All rights reserved.
*
* This program can be copied or used in any manner desired.
*/

import net.windward.datasource.dom4j.Dom4jDataSource;
import net.windward.env.SystemWrapper;
import net.windward.xmlreport.ProcessPdf;
import net.windward.xmlreport.ProcessPdfAPI;
import net.windward.xmlreport.ProcessReport;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.util.HashMap;
import java.util.Map;

public class InputParametersSample
{
    public static void main(String[] args) throws Exception
    {
        // Initialize Fluent
        ProcessReport.init();

        // Specify output file
        File fileReport = new File("Samples/Variable Example test.pdf");
        FileOutputStream out = new FileOutputStream(fileReport);

        // Specify our template
        String filename = new File("Samples/Variable Example.docx").getAbsolutePath();
        FileInputStream rtf = new FileInputStream(filename);

        //open our xml file and create a datasource
        InputStream dataSourceStream = new FileInputStream(new File("Samples/Windward Trucking 2 - Data.xml").getAbsolutePath());
        Dom4jDataSource data = new Dom4jDataSource(dataSourceStream);

        // Create a report process
        System.out.println("Generating report...");
        ProcessPdfAPI proc = new ProcessPdf( rtf, out );

        // set the subject
        proc.setSubject("An Acme, Inc. Report");

        // parse the template file
        proc.processSetup();

        //This is where we pass in the parameters to the datasource
        Map<String,Object> map = new HashMap<String,Object>();
        map.put("var1", "10537");

        //the actual function that gives the datasource our parameters
        proc.setParameters(map);

        // merge a sql database with the report
        proc.processData(data, "FD");

        // generate the final report
        proc.processComplete();

        // ensure everything is written out to the stream
        dataSourceStream.close();

		out.close();
		rtf.close();

        System.out.println("Finished generating report " + fileReport.getAbsolutePath());
    }
}