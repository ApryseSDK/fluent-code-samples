# Batch Processing With .NET RESTful Client Sample

## Overview
This sample contains the files necessary to demonstrate batch processing with a JSON data source. The JSON file provided in the Data folder in the sample directory contains a list of records, with each entry corresponding to an order. The sample will parse this list of records, and create a request to process a simple Invoice template for each record. It will then save the completed documents in a folder called "GeneratedDocs" located in the root folder of the project.

## Usage
To run this sample:

1. Open the .sln file
2. Open the App.config file and insert your RESTful Engine URL in the "restful-engine-url" property
3. Also in the App.config file and insert your license key in the "license" property
    1. If you don't have a license key, leave it blank and output will generate with a watermark
4. In the "Build" dropdown, select "Rebuild Solution"
5. Save and click *Start*