# Generate Any Document .NET Command Line Sample

## Overview
This sample is a command line project which exposes nearly all document generation features of Fluent using a few command line arguments.

## Running the Project
1. Add your license in the App.config file
   1. If you don't have a license, you can leave it blank and output will generate with a watermark
2. In Visual Studio, right click ont he GenerateAnyDocumentCommandLine C# Project and select *Properties*
3. In the Properties window, navigate to the *Debug* tab on the left side bar
4. In the Debug window, you will enter values into the *Command line arguments* text field.
5. Follow usage below to get the values to enter in the *Command line arguments* field.

## Simple Example Usage
At the simplest level you will specify a template, and output file and a data source if necessary.

This can will be specified like so:
```
[template_file] [output_file] [datasource_type]:[datasource_name] [datasource_connection_string]
```

For example, with the provided template and data source in the *Samples* directory, the arguments would look like:

```
"../../../Samples/InvoiceTemplate.docx" "../../../Samples/output.pdf" -xml "../../../Samples/Order.xml"
```

In this example, no name was specified for the data source since the template does not contain a specific name for the datasource.

## Full Usage
For the full usage, you can run this project with no command line arguments specified to get the full usage displayed.

Here is a breakdown of the full usage:

```
GenerateAnyDocumentCommandLine template_file output_file [-basedir path] [-xml xml_file | -sql connection_string | -sforce | -oracle connection_string | -ole oledb_connection_string] [key=value | ...]

       The template file can be a docx, pptx, or xlsx file.
       The output file extension determines the report type created:
           output.csv - SpreadSheet CSV file
           output.docm - Word DOCM file
           output.docx - Word DOCX file
           output.htm - HTML file with no CSS
           output.html - HTML file with CSS
           output.pdf - Acrobat PDF file
           output.pptm - PowerPoint PPTM file
           output.pptx - PowerPoint PPTX file
           output.prn - Printer where "output" is the printer name
           output.rtf - Rich Text Format file
           output.txt - Ascii text file
           output.xhtml - XHTML file with CSS
           output.xlsm - Excel XLSM file
           output.xlsx - Excel XLSX file
       -basedir - will set the base directory to this.
       -data filename.xml - will write data.xml to this filename.
       -embed - will embed data.xml in the generated report. DOCX, PDF, PPTX, & XLSX only.
       -launch - will launch the report when complete.
       -performance:123 - will run the report 123 times.
            output file is used for directory and extension for reports
       -cache - will cache template & datasources, will write output to memory stream. Only used with -performance.
       -threads:4 - will create 4 threads when running -performance.
       -verify:N - turn on the error handling and verify feature where N is a number: 0 (none) , 1 (track errors), 2 (verify), 3 (all).  The list of issues is printed to the standard error.
       -version=9 - sets the template to the passed version (9 in this example).
       encoding=UTF-8 (or other) - set BEFORE datasource to specify an encoding.
       locale=en_US - set the locale passed to the engine.
       pod=pod_filename - set a POD file (datasets).
       username=user password=pass - set BEFORE datasource for database connections.
       The datasource is identified with a pair of parameters
           -json filename - passes a JSON file as the datasource
               filename can be a url/filename or a connection string
           -odata url - passes a url as the datasource accessing it using the OData protocol
           -sforce - password should be password + security_token (passwordtoken)
           -xml filename - XPath 3.1 passes an xml file as the datasource
                -xml xmlFilename=schema:schemaFilename - passes an xml file and a schema file as the datasource
               filename can be a url/filename or a connection string
           -xpath filename - [deprecated] uses the old XPath 1.0 datasource.
                -xml xmlFilename=schema:schemaFilename - passes an xml file and a schema file as the datasource
               filename can be a url/filename or a connection string
           -db2 connection_string ex: server=localhost;database=SAMPLE;Uid=test;Pwd=pass;
           -ibm_informix_.net_data_provider_12.1.0 connection_string ex: connection_string
           -mysql connection_string ex: server=mysql.windwardreports.com;database=sakila;user id=demo;password=demo;
           -odbc connection_string ex: Driver={Sql Server};Server=localhost;Database=Northwind;User ID=test;Password=pass;
           -odbc connection_string ex: Driver={Sql Server};Server=localhost;Database=Northwind;User ID=test;Password=pass;
           -oledb connection_string ex: Provider=sqloledb;Data Source=localhost;Initial Catalog=Northwind;User ID=test;Password=pass;
           -oledb connection_string ex: Provider=sqloledb;Data Source=localhost;Initial Catalog=Northwind;User ID=test;Password=pass;
           -oracle connection_string ex: Data Source=oracle.windwardreports.com:1521/HR;Persist Security Info=True;Password=HR;User ID=HR
           -sql connection_string ex: Data Source=mssql.windwardreports.com;Initial Catalog=Northwind;user=demo;password=demo;
           -sql connection_string ex: Data Source=mssql.windwardreports.com;Initial Catalog=Northwind;user=demo;password=demo;
               if a datasource is named you use the syntax -type:name (ex: -xml:name filename.xml)
       You can have 0-N key=value pairs that are passed to the datasource Map property
            If the value starts with I', F', or D' it parses it as an integer, float, or date(yyyy-MM-ddThh:mm:ss)
            If the value is * it will set a filter of all
            If the value is "text,text,..." it will set a filter of all
```