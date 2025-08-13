package com.apryse.fluent;

import WindwardRestApi.Api.WindwardClient;
import WindwardRestApi.Model.Document;
import WindwardRestApi.Model.Template;
import WindwardRestApi.Model.VersionInfo;
import WindwardRestApi.Model.Xml_10DataSource;
import com.apryse.fluent.util.PropertiesUtil;

import java.io.File;
import java.io.FileOutputStream;
import java.nio.file.Files;
import java.util.Collections;

/// <summary>
/// Sample code to generate a document using the Fluent v2 RESTful Engine Client in Java
/// </summary>
public class GenerateDocument {

    // set your RESTful Engine URL in the application.properties with the key "restful-engine-url"
    private static final String RESTFUL_ENGINE_URL = PropertiesUtil.getProperty("restful-engine-url");

    private static WindwardClient client;

    public static void main(String[] args) throws Exception {

        /**
         * Step 1: INITIALIZE THE RESTFUL CLIENT AND SET UP THE TEMPLATE AND DATA SOURCE
         * --------------------------------------------------------------------------------------------
         */
        System.out.println("Initializing Fluent RESTful Client");

        // Initialize the RESTful Client
        client = new WindwardClient(RESTFUL_ENGINE_URL);

        // Pass in the License Key for the RESTful Engine (if you have one) in the application.properties file under the key "license"
        // If you don't have a license key, you can leave it blank and it will produce output with a watermark.
        client.setLicense(PropertiesUtil.getProperty("license"));

        // Display the current engine version of your Fluent RESTful Engine
        VersionInfo versionInfo = client.getVersion();
        System.out.println("Fluent RESTful Engine Version: " + versionInfo.getEngineVersion());

        // Get our template and data source as byte arrays to pass to the RESTful Engine Client
        byte[] templateBytes = Files.readAllBytes(new File("files\\InvestmentFactSheet.docx").toPath());
        byte[] dataSourceBytes = Files.readAllBytes(new File("files\\InvestmentFactSheet.xml").toPath());

        // Create our template object that will output to PDF format
        Template template = new Template(Template.OutputFormatEnum.PDF, templateBytes, Template.FormatEnum.DOCX);

        // Create our data source object for our template
        Xml_10DataSource dataSource = new Xml_10DataSource("InvestmentFactSheet", dataSourceBytes, null);

        // Add the data source to the template
        template.setDatasources(Collections.singletonList(dataSource));

        /**
         * Step 2: GENERATE THE DOCUMENT
         * --------------------------------------------------------------------------------------------
         */
        System.out.println("Generating document...");

        // Post our template generation request to the RESTful Engine
        Document document = client.postDocument(template);

        // Wait until the document generation is complete before proceeding
        while (true) {
            Thread.sleep(100);
            int status = client.getDocumentStatus(document.getGuid());
            if (status == 302) {
                // The document generation is complete, we can now proceed to retrieve the generated document
                break;
            }
            else if (status == 201 || status == 202 || status == 404) {
                // The document generation is still in progress, continue waiting
                continue;
            }
            else {
                // Potentially have an error, proceed to retrieve the document to get error details
                System.out.println("Error retrieving document. Status code: " + status);
                break;
            }
        }


        /**
         * Step 3: RETRIEVE THE GENERATED DOCUMENT
         * --------------------------------------------------------------------------------------------
         */
        System.out.println("Retrieving the generated document...");
        document = client.getDocument(document.getGuid());

        // Write out the generated document to a file (ensure the file format matches the output format specified in the template)
        File outputFile = new File("files\\Output.pdf");
        outputFile.createNewFile();
        try (FileOutputStream fos = new FileOutputStream(outputFile)) {
            fos.write(document.getData());
        } catch (Exception e) {
            System.err.println("Failed to write generated document to file: " + e.getMessage());
            return;
        }

        System.out.println("Generated document saved to: " + outputFile.getAbsolutePath());
    }
}
