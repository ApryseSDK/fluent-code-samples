# Embedded .NET Engine DataSet Sample

## Overview
This sample project demonstrates how to create Flunet Datasets and utilize them when generating a document.

Datasets are analogous to SQL views.  They help simplify queries by only exposing a subset of the datasource and can then be treated as their own datasource.  This is especially helpful in the Fluent Designer when designing a template.  This sample shows that this can also be achieved through the .NET engine by creating a variety of Datasets that are used to generate output.

## Usage
Use the following steps to build and run the project.

1. Open the .sln file of the sample project you would like to run
2. Open the App.config file and insert your license key in the "license" property
    1. If you don't have a license key, leave it blank and output will generate with a watermark
3. In the "Build" dropdown, select "Rebuild Solution"
4. Once the build has finished successfully click "Start"

## More Information
More information about Fluent's Datasets can be discovered [here](https://fluent.apryse.com/documentation/designer-guide/FAQ/howToUseDatasets) in our documentation.