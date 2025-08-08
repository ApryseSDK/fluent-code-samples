## RunUsingDatasets Sample
This sample Java program demonstrates how to generate a report using the Windward Reports Java Engine with both XML and SQL Server data sources, including the use of DataSets for filtering and subsetting data.

The output is a PDF report generated from a DOCX template.

## Prerequisites
Before running this sample, ensure you have the following:

1. Windward Reports Java Engine
   You must have the Windward Java engine JAR files available and on your classpath.

2. Microsoft SQL Server JDBC Driver
   This sample uses SQL Server as a data source. Download the Microsoft JDBC Driver for SQL Server and include the appropriate .jar file in your classpath (e.g., mssql-jdbc-12.6.1.jre11.jar).

3. Java
   Ensure you are using Java 8 or later.

## Folder Structure
``` pgsql
.
├── RunUsingDatasets.java             # Main sample file
├── files/
│   ├── Sample Dataset Template.docx  # Report template (Word document)
│   ├── SouthWind.xml                 # XML data file
│   └── SouthWind.xsd                 # XML schema file
└── out/
└── Sample Dataset Report.pdf     # Output folder for the generated PDF
```

## Running the Project
1. Open the project in Intellij
2. Insert your license in the WindwardReports.properties file
   1. If you don't have a license you can leave it blank and a watermark will be present in output
3. In the "Build" dropdown, select "Build"
4. Run the sample with the *Run* button

You may modify these files to suit your own test data and template.

## What the Sample Does
Step-by-Step Behavior
1. Loads the Word template from files/Sample Dataset Template.docx.

2. Creates a ProcessPdf instance to generate a PDF output.

3. Loads XML data from SouthWind.xml and its schema SouthWind.xsd.

4. Applies XPath-based DataSet filters:

- Employees with EmployeeID < 5

- Customers whose names start with A

5. Connects to SQL Server using:

``` makefile
jdbc:sqlserver://mssql.windward.net;DatabaseName=Northwind
User: demo
Password: demo
```
6. Applies SQL-based DataSet filters:

- Employees with EmployeeID > 5

- Customers whose names start with B

7. Maps all datasets into a unified data context.

8. Processes the report using the Windward engine.

9. Outputs the result as a PDF to ```out/Sample Dataset Report.pdf```.

10. Launches the PDF automatically after generation.

## Notes
If you encounter the error ClassNotFoundException: com.microsoft.sqlserver.jdbc.SQLServerDriver, it means the JDBC driver is not properly added to your classpath.

The SQL Server used in this sample (mssql.windward.net) is a demo server. You can modify the connection string to use your own server and credentials.

This code uses Java reflection to open the generated file with the default system viewer. If that fails, it falls back to platform-specific commands (rundll32 on Windows, open on macOS/Linux).

## Customization
You can change:

- The report template

- The XML and XSD files

- The SQL query and connection info

- The XPath expressions used in the dataset filters

- Add a License Key to the WindwardReports.properties file in the root directory to remove watermarks from output

Make sure that the template contains the required tags and regions matching the dataset names defined in the code.