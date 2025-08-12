# Fluent Java RESTful Client v2 Sample

## Overview
This sample project demonstrates how to use the v2 Java RESTful Client to accomplish:
1. Generating a document
2. Retrieving a template's metrics
3. Retrieving a template's tag tree

This project consists of 3 main classes that can each be individually ran to show each piece of functionality.

## Requirements
* [Java 8 Installation](https://www.oracle.com/java/technologies/downloads/#java8-windows)
* [Maven Installation](https://maven.apache.org/download.cgi) (If you need instructions, follow the ones listed [here](https://maven.apache.org/install.html))
* A RESTful Engine Installation
  * For instructions on setting up the RESTful Engine with Docker, please refer to the following [article](https://fluent.apryse.com/documentation/engine-guide/Fluent%20RESTful%20Engines/fluentJavaRESTDocker)
  * For instructions on setting up the RESTful Engine with Tomcat, please refer to the following [article](https://fluent.apryse.com/documentation/engine-guide/Fluent%20RESTful%20Engines/SettingUptheJavaRESTfulEngine)

## Intellij Usage
Use the following steps to build and run the sample through Intellij

1. Open the project folder in Intellij
2. Open the application.properties file and insert your RESTful Engine URL in the "restful-engine-url" property
3. Also in the application.properties file and insert your license key in the "license" property
   1. If you don't have a license key, leave it blank and output will generate with a watermark
4. In the "Build" dropdown, select "Rebuild Project"
5. Open the file you would like to run (GenerateDocument.java, RetrieveMetrics.java, or RetrieveTagTree.java)
6. Click the "Run" button next to the class name in the file

## Command Line Usage
The commands for command line usage for each sample can be found in their respective .bat file in the root of the project.

The .bat files can be ran as well.

## Additional Information
You can find additional information about the v2 Java RESTful Clients [here](https://fluent.apryse.com/documentation/engine-guide/Fluent%20RESTful%20Engines/RESTfulJavaClient).