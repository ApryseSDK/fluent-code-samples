# Generate Any Document Command Line Sample

## Overview
This sample is a command line project which exposes nearly all document generation features of Fluent using a few command line arguments.

## Running the Project
1. Add your license in the WindwardReports.properties file
   1. If you don't have a license, you can leave it blank and output will generate with a watermark
2. In the top right corner of Intellij, click the Project dropdown by the Run button and select *Edit Configuration*
3. In the Edit Configuration window, you will enter values into the *Program Arguments* text field.
4. Follow usage below to get the values to enter in the *Program Arguments* field.

## Simple Example Usage
At the simplest level you will specify a template, and output file and a data source if necessary.

This can will be specified like so:
```
[template_file] [output_file] [datasource_type]:[datasource_name] [datasource_connection_string]
```

For example, with the provided template and data source in the *samples* directory, the arguments would look like:

```
"samples/Fluent_Financial_Report_Template.docx" "samples/output.pdf" -dom4j:WRFINANCIAL "samples/Fluent_Financial_Data.xml"
```

In this example, no name was specified for the data source since the template does not contain a specific name for the datasource.

## Full Usage
For the full usage, you can run this project with no command line arguments specified to get the full usage displayed.

Here is a breakdown of the full usage:

```
GenerateAnyDocumentCommandLine template_file output_file [-basedir path] [-xml xml_file | -sql connection_string | -oracle connection_string | -ole oledb_connection_string] [licenseKey=value | ...]
       
       The template file can be a docx, pptx, or xlsx file.
       The output file extension determines the report type created:
           output.csv - SpreadSheet CSV file
           output.docx - Word 2007+ DOCX file
           output.htm - HTML file with no CSS
           output.html - HTML file with CSS
           output.pdf - Acrobat PDF file
           output.pptx - PowerPoint 2007+ PPTX file
           output.prn - Printer where "output" is the printer name
           output.rtf - Rich Text Format file
           output.txt - Ascii text file
           output.xhtml - XHTML file with CSS
           output.xlsx - Excel 2007+ XLSX file
           output.xlsm - Excel 2007+ macro enabled XLSM file
       -basedir c:\test - sets the datasource base directory to the specified folder (c:\test in this example)
       -data filename.xml - will write data.xml to this filename.
       -embed - will embed data.xml in the generated report. DOCX, PDF, PPTX, & XLSX only.
       -launch - will launch the report when complete.
       -performance:123 - will run the report 123 times.
            output file is used for directory and extension for reports
       -cache - will cache template & datasources, will write output to memory stream. Only used with -performance
       -threads:4 - will create 4 threads when running -performance.
       -verify:N - turn on the error handling and verify feature where N is a number: 0 (none) , 1 (track errors), 2 (verify), 3 (all).  The list of issues is printed to the standard error.
       -version=9 - sets the template to the passed version (9 in this example)
       encoding=UTF-8 (or other) - set BEFORE datasource to specify an encoding
       locale=en_US - set the locale passed to the engine.
       pod=pod_filename - set a POD file (datasets)
       username=user password=pass - set BEFORE datasource for database connections
       The datasource is identified with a pair of parameters (the [prepend] part is prepended to the connection string
           -json filename - passes a JSON file as the datasource
                filename can be a url/filename or a connection string
           -odata url - passes a url as the datasource accessing it using the OData protocol
           -sforce - password should be password+securitytoken
           -xml filename - XPath 3.1 passes an xml file as the datasource
                -xml xmlFilename=schema:schemaFilename - passes an xml file and a schema file as the datasource
                filename can be a filename or a connection string
           -dom4j filename - [deprecated] uses the old XPath 1.0 datasource
                -dom4j xmlFilename=schema:schemaFilename - passes an xml file and a schema file as the datasource
                filename can be a filename or a connection string
           -[xml|sql|...]:name names this datasource with name
                     must come BEFORE each -xml, -sql, ... part
       You can have 0-N key=value pairs that are passed to the datasource Map property
            If the value starts with I', F', or D' it parses it as an integer, float, or date(yyyy-MM-ddThh:mm:ss)
                example  date="D'1996-08-29"
            If the value is * it will set a filter of all
            If the value is \"text,text,...\" it will set a filter of all
```
