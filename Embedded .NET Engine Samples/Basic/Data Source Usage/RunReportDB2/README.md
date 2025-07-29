# Embedded .NET Engine XML Data Source Sample

## Overview
This sample project demonstrates generating a document from a template that uses an XML datasource.

Note that we do not have a public DB2 sample database so you will need to enter your own template and data source connection info.  We provide the skeleton code in this sample to accomplish that.

## Usage
Use the following steps to build and run the project.

1. Open the .sln file of the sample project you would like to run
2. Open the App.config file and insert your license key in the "license" property
    1. If you don't have a license key, leave it blank and output will generate with a watermark
3. Insert your template connection string and data source connection string in the code.
4. In the "Build" dropdown, select "Rebuild Solution"
5. Once the build has finished successfully click "Start"