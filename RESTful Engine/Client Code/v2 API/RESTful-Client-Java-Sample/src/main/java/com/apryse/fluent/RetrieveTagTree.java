package com.apryse.fluent;

import WindwardRestApi.Api.WindwardClient;
import WindwardRestApi.Model.*;
import com.apryse.fluent.util.PropertiesUtil;

import java.io.File;
import java.io.FileOutputStream;
import java.nio.file.Files;
import java.util.Collections;

/// <summary>
/// Sample code to retrieve a template's tag tree using the Fluent v2 RESTful Engine Client in Java
/// </summary>
public class RetrieveTagTree {

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
         * Step 2: REQUEST THE TEMPLATE TAG TREE
         * --------------------------------------------------------------------------------------------
         */
        System.out.println("Requesting template tag tree...");

        // Post our template metrics request to the RESTful Engine
        TagTree tagTree = client.postTagTree(template);

        // Wait until the tag tree request is complete before proceeding
        while (true) {
            Thread.sleep(100);
            int status = client.getTagTreeStatus(tagTree.getGuid());
            if (status == 302) {
                // The tag tree request is complete, we can now proceed to retrieve the tagTree
                break;
            }
            else if (status == 201 || status == 202 || status == 404) {
                // The tag tree request is still in progress, continue waiting
                continue;
            }
            else {
                // Potentially have an error, proceed to retrieve the tag tree to get error details
                System.out.println("Error retrieving template tag tree. Status code: " + status);
                break;
            }
        }


        /**
         * Step 3: RETRIEVE THE TEMPLATE TAG TREE
         * --------------------------------------------------------------------------------------------
         */
        System.out.println("Retrieving the template tag tree...");
        tagTree = client.getTagTree(tagTree.getGuid());

        // Write out the template tag tree to a file
        File outputFile = new File("files\\TagTree.xml");
        outputFile.createNewFile();
        try (FileOutputStream fos = new FileOutputStream(outputFile)) {
            fos.write(tagTree.getXml());
        } catch (Exception e) {
            System.err.println("Failed to write tag tree to file: " + e.getMessage());
            return;
        }

        System.out.println("Tag Tree saved to: " + outputFile.getAbsolutePath());
    }
}
