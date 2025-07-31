# SemaphoreSample

This is a C# console application that demonstrates how to use a semaphore to limit concurrent access to Windward Fluent report generation when operating under a licensed thread limit.

## Overview

The application spawns multiple threads, each attempting to generate a report using the Windward Fluent engine. A semaphore is used to restrict the number of concurrent report generations based on a configurable value.

## Features

- Limits the number of concurrent report processes to avoid exceeding licensed thread limits
- Uses `Semaphore` for thread synchronization
- Demonstrates correct usage of the Windward Fluent `.NET` API with `ReportDocx`
- Processes DOCX reports from XML data sources
- Supports configuration of maximum concurrent threads via `App.config`

## Prerequisites

- .NET Framework
- Windward Fluent Engine libraries referenced (`net.windward.api.csharp`, `WindwardInterfaces`)
- A valid Fluent license
- Input template and XML data files

## File Structure

```
/files/
  Fluent Trucking 2 - Template.docx
  Fluent Trucking 2 - Data.xml

/out/
  Report_0.docx
  Report_1.docx
  ...
  Report_9.docx

Program.cs
MyRunReport.cs
App.config
```

## Configuration

Set the maximum number of concurrent threads in `App.config`:

```xml
<appSettings>
  <add key="NumberThreads" value="3"/>
</appSettings>
```

## How to Run

1. Ensure required input files are placed under the `files` directory.
2. Build and run the application.
3. The application will:
   - Initialize the Fluent engine
   - Spawn multiple threads, each creating a report
   - Enforce concurrency limit using a semaphore
   - Write completed reports to the `out` directory
   4. (optional) Add your license key to the App.Config by adding this section nested in the <configuration>:

  <WindwardReports>
    <add key="license" value="[[LICENSE]]"/>
  </WindwardReports>

  replace [[LICENSE]] with your license key. Leaving this out will result in watermarked output.

## Notes

- Each report thread will sleep for 10 seconds after setup to simulate concurrency pressure.
- `Semaphore.WaitOne()` and `Semaphore.Release()` are used to safely guard access to the engine.
- Reports are created using `ReportDocx`, reading from a DOCX template and an XML data file.

## License

This sample assumes a valid Windward Fluent license and is provided for educational and demonstration purposes only.