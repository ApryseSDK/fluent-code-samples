# .NET RESTful Client v2 Sample 

## Overview
This sample project demonstrates how to use the v2 .NET RESTful Client with C# to accomplish:
1. Generating a document
2. Retrieving a template's metrics
3. Retrieving a template's tag tree

This project consists of 3 main classes that can each be individually ran to show each piece of functionality.

## Requirements
* [.NET Installation](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* A RESTful Engine Installation
    * For instructions on setting up the RESTful Engine with Docker, please refer to the following [article](https://fluent.apryse.com/documentation/engine-guide/Fluent%20RESTful%20Engines/fluentJavaRESTDocker)
    * For instructions on setting up the RESTful Engine with Tomcat, please refer to the following [article](https://fluent.apryse.com/documentation/engine-guide/Fluent%20RESTful%20Engines/SettingUptheJavaRESTfulEngine)

## Visual Studio Usage
Use the following steps to build and run the sample through Visual Studio.

1. Open the .sln file
2. Open the App.config file and insert your RESTful Engine URL in the "restful-engine-url" property
3. Also in the App.config file and insert your license key in the "license" property
    1. If you don't have a license key, leave it blank and output will generate with a watermark
3. In the "Build" dropdown, select "Rebuild Solution"
4. To select which sample file you would like to run, in Visual Studio 2022:
    1. Open the project properties by right clicking on the project in the *Solution Explorer* and select *Properties*
    2. In the side tag *Application > General* find the portion called *Startup object*
    3. In that dropdown, select the sample class you would like to run
4. Save and click *Start*

## Command Line Usage
Use the following steps to build and run the sample from the command line.

1. Open a command prompt in the root directory where the .csproj file is located
2. To build the project for a specific file run the following commands:
    1. For GenerateDocument
    ```
    dotnet build RESTful-Client-.NET-Sample.csproj -p:StartupObject=RESTful_Client_.NET_Sample.GenerateDocument -t:Rebuild
    ```
    2. For RetrieveMetrics
    ```
    dotnet build RESTful-Client-.NET-Sample.csproj -p:StartupObject=RESTful_Client_.NET_Sample.RetrieveMetrics -t:Rebuild
    ```
    3. For RetrieveTagTree
    ```
    dotnet build RESTful-Client-.NET-Sample.csproj -p:StartupObject=RESTful_Client_.NET_Sample.RetrieveTagTree -t:Rebuild
    ```
3. To run the project, use the command `dotnet run`
    1. Alternatively you can run the .dll or .exe file that are produced in the *bin/Debug/netX.0/* folder where *X* is your .NET version

## Additional Information
You can find additinal information about the v2 .NET RESTful Clients [here](https://fluent.apryse.com/documentation/engine-guide/Fluent%20RESTful%20Engines/RESTfulNetClient).