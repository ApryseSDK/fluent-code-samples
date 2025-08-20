# Legacy RESTful Engine — S3 Repository Plugin (Sample)

⚠️ **Deprecation Notice**  
The **.NET RESTful Engine** is **deprecated** and no longer actively maintained.  
We strongly recommend upgrading to the **Java RESTful Engine**, which is the current and supported engine moving forward.  
This sample exists only for legacy support scenarios and should not be used for new development.

---

This repository is a **minimal, public-ready sample** that shows how to back the Fluent RESTful Engine with **Amazon S3**. It demonstrates wiring a low-level S3 repository into a higher-level storage manager that the Engine can load as a plugin. The code uses the AWS SDK for .NET (S3; and optionally related helpers) while keeping the plugin surface small and easy to adapt.

---

## Files & Folders

- **SampleRESTfulS3Plugin.sln**  
  Visual Studio solution file for opening the project.

- **SampleRESTfulS3Plugin.csproj**  
  Project file defining targets, references, and build configuration.

- **app.config**  
  Configuration **template** with placeholders for AWS Region, Bucket, and (if you choose) credentials/prefixes. At build, this becomes `SampleRESTfulS3Plugin.dll.config`. Do not commit real secrets.

- **packages.config**  
  NuGet dependency list. Consumers restore packages before building.

- **Properties/**
  - **AssemblyInfo.cs** – Assembly metadata (title, version, etc.).

- **Models/**
  - **JobRequestData.cs** – Data contract used by the Engine to describe a job/request the plugin will store or retrieve.
  - **JobInfoEntity.cs** – POCO representing job metadata/status used internally by the sample.

- **Repository/**
  - **S3Repository.cs** – Concrete repository that adapts Amazon S3 to Fluent’s repository expectations (leveraging `FluentRepository` types already provided by the Engine). Handles low-level S3 CRUD and object-key management.

- **S3Storage/**
  - **S3StorageManager.cs** – **Public plugin entry point.** Orchestrates operations using `S3Repository` (e.g., saving documents, retrieving by key, deleting). This is the class that gets compiled into the plugin DLL and loaded by the Engine.

> Note: This repo ships **source only**. Consumers add their credentials/bucket settings, restore NuGet packages, and build to produce the plugin DLL.

---

## Documentation

For end-to-end instructions on configuring, building, and loading this plugin with the Fluent RESTful Engine, see the [official guide](https://fluent.apryse.com/documentation/engine-guide/Fluent%20RESTful%20Engines/NetRestSotragePlugin).
