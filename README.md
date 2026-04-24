# Fluent Code Samples

A cross-language sample catalog for [Apryse Fluent](https://apryse.com/fluent) document generation. This repository contains working examples for integrating the Fluent engine into .NET and Java applications, as well as client and plugin samples for the RESTful engine.

## Embedded .NET Engine Samples

C# projects demonstrating how to embed the Fluent engine directly in a .NET application.

### Basic

| Sample | Description |
|--------|-------------|
| [Data Source Usage](Embedded%20.NET%20Engine%20Samples/Basic/Data%20Source%20Usage) | Individual projects showing how to generate documents from XML, SQL, MySQL, Oracle, DB2, JSON, and OData data sources |
| [Input Parameters](Embedded%20.NET%20Engine%20Samples/Basic/Input%20Parameters) | How to pass input parameters to a template at generation time |

### Advanced

| Sample | Description |
|--------|-------------|
| [Custom Functions](Embedded%20.NET%20Engine%20Samples/Advanced/CustomFunctions) | Build and register a custom function DLL that can be called from within a template |
| [Custom Callbacks](Embedded%20.NET%20Engine%20Samples/Advanced/CustomCallbacks) | Intercept, inspect, and modify select expressions during document processing |
| [Data Mode](Embedded%20.NET%20Engine%20Samples/Advanced/DataModeSample) | Use the Data Mode feature to emit a structured data file alongside the generated document |
| [Using Datasets](Embedded%20.NET%20Engine%20Samples/Advanced/UsingDatasets) | Define datasets programmatically to provide filtered, SQL view-like data subsets to a template |
| [Generate Any Document (CLI)](Embedded%20.NET%20Engine%20Samples/Advanced/GenerateAnyDocumentCommandLine) | Command-line utility that drives document generation with a wide range of options |
| [Semaphore](Embedded%20.NET%20Engine%20Samples/Advanced/Semaphore) | Use a semaphore to limit concurrent generation threads and stay within license limits |

---

## Embedded Java Engine Samples

Java projects demonstrating how to embed the Fluent engine directly in a Java application. These samples mirror the .NET samples and cover the same feature set.

### Basic

| Sample | Description |
|--------|-------------|
| [Data Source Usage](Embedded%20Java%20Engine%20Samples/Basic/Data%20Source%20Usage) | Individual projects showing how to generate documents from XML, SQL, MySQL, Oracle, DB2, JSON, OData, and Salesforce data sources |
| [Input Parameters](Embedded%20Java%20Engine%20Samples/Basic/Input%20Parameters) | How to pass input parameters to a template at generation time |

### Advanced

| Sample | Description |
|--------|-------------|
| [Custom Functions](Embedded%20Java%20Engine%20Samples/Advanced/CustomFunctions) | Implement and register a custom function that can be called from within a template |
| [Custom Callbacks](Embedded%20Java%20Engine%20Samples/Advanced/CustomCallbacks) | Callback class to approve or modify select expressions during processing |
| [Data Mode](Embedded%20Java%20Engine%20Samples/Advanced/DataModeSample) | Java equivalent of the Data Mode feature |
| [Using Datasets](Embedded%20Java%20Engine%20Samples/Advanced/UsingDatasets) | Mixed XML and SQL dataset filtering with mapped data contexts |
| [Generate Any Document (CLI)](Embedded%20Java%20Engine%20Samples/Advanced/GenerateAnyDocumentCommandLine) | Command-line utility for document generation |
| [Semaphore](Embedded%20Java%20Engine%20Samples/Advanced/SemaphoreSample) | Thread-limited parallel generation using a semaphore |

---

## RESTful Engine

Examples for working with the Fluent RESTful engine, organized into client code and server-side plugins.

### Client Code

#### v2 API (current)

| Sample | Language | Description |
|--------|----------|-------------|
| [RESTful-Client-.NET-Sample](RESTful%20Engine/Client%20Code/v2%20API/RESTful-Client-.NET-Sample) | C# | Generate documents, retrieve metrics, and retrieve the tag tree |
| [RESTful-Client-.NET-BatchProcessing](RESTful%20Engine/Client%20Code/v2%20API/RESTful-Client-.NET-BatchProcessing) | C# | Process a batch of JSON records into multiple generated documents |
| [RESTful-Client-Java-Sample](RESTful%20Engine/Client%20Code/v2%20API/RESTful-Client-Java-Sample) | Java | Generate documents, retrieve metrics, and retrieve the tag tree |
| [RESTful-Client-JavaScript-Sample](RESTful%20Engine/Client%20Code/v2%20API/RESTful-Client-JavaScript-Sample) | JavaScript | Client-side document generation via the v2 API |
| [RESTful-Client-Python-Sample](RESTful%20Engine/Client%20Code/v2%20API/RESTful-Client-Python-Sample) | Python | Generate documents, retrieve metrics, and retrieve the tag tree |
| [RESTful-Client-FrontEnd-Display-With-WebViewer](RESTful%20Engine/Client%20Code/v2%20API/RESTful-Client-FrontEnd-Display-With-WebViewer) | Node.js | Generate a document via the RESTful API and display the output in a browser using WebViewer |

#### v1 Legacy API (deprecated)

The [v1 Legacy API](RESTful%20Engine/Client%20Code/v1%20Legacy%20API) folder contains samples in C#, Java, PHP, Python, Ruby, TypeScript, .NET Core, and .NET Framework. These are no longer maintained — new projects should use the v2 API.

### Plugins

Server-side plugins that extend the RESTful engine with custom storage backends or post-processing logic.

#### Java RESTful (current)

| Sample | Description |
|--------|-------------|
| [Java-RESTful-S3-Repository-Sample](RESTful%20Engine/Plugins/Java%20RESTful/Java-RESTful-S3-Repository-Sample) | `IRepository` plugin that stores and retrieves templates and output from AWS S3 |
| [Custom Processing Sample](RESTful%20Engine/Plugins/Java%20RESTful/Custom%20Processing%20Sample) | Post-processor plugin for custom handling of generated document output |

#### Legacy .NET RESTful (deprecated)

| Sample | Description |
|--------|-------------|
| [NET-RESTful-Azure-Repository-Sample](RESTful%20Engine/Plugins/Legacy%20NET%20RESTful/NET-RESTful-Azure-Repository-Sample) | `IRepository` plugin backed by Azure Blob Storage |
| [NET-RESTful-S3-Repository-Sample](RESTful%20Engine/Plugins/Legacy%20NET%20RESTful/NET-RESTful-S3-Repository-Sample) | `IRepository` plugin backed by AWS S3 |
| [Custom Processing Sample](RESTful%20Engine/Plugins/Legacy%20NET%20RESTful/Custom%20Processing%20Sample) | Legacy .NET custom post-processor plugin |

---

## Getting Started

Each sample folder contains its own `README.md` with prerequisites, setup steps, and instructions for running that specific sample. Start there for the sample you are interested in.

General prerequisites by platform:

- **.NET samples** — .NET SDK, NuGet
- **Java samples** — JDK 8+, Maven
- **RESTful client samples** — A running Fluent RESTful engine instance; language-specific runtime (Node.js, Python, etc.)
- **RESTful plugin samples** — A Fluent RESTful engine deployment to host the plugin
