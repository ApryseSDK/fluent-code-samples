# Windward RESTful V2 Python Client Sample

  
## Setup
NOTE: You NEED to have Python V 3.8.6 (64bit).

Next, locate the `congig.ini` file in the project root. This file contains all the config properties needed to run these samples. Open the file and:
1. Set your license
1. Set the address of your RESTful server


## Running the samples

We have three samples in this project:
1. GenerateDocument.py
    - Shows you how to process a document and get output
1. RetrieveMetrics.py
    - Shows you how to get the templates metrics
1. RetrieveTagTree.py
    - Shows you how to get the templates tag tree

All these python files are located in `/src/fluent_demo/` if you want to explore the code.

To run these projects all you need to do is:
1. Navigate to the `/scripts/` directory
1. Double click on the corisponding `.bat` file for the project you want to run. Everrything is setup to work out the box.

We offer a handful of sample templates located in the `/files/` directory. The project is setup to run the "Manufacturing.docx" template (with XML datasource) by default. If you want to run another sample:
1. Open the config.ini file
1. Change the `template_file` and `data_file` values to the sample you want to run
    - Make sure you make the appropriate code changes in the project you are running

To see the object schemas used in this client, check out our [Swagger Documentation](https://app.swaggerhub.com/apis/Windward-Studios/fluent-rest_full_api/2.0)
