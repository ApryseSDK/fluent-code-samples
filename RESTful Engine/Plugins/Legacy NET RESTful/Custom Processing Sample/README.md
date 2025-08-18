# Fluent .NET RESTful Custom Processing Sample
This is a sample project to see how the custom processing works in the Legacy .NET RESTful engine.
In this sample, we are removing the data from the document, so when you get the document after processing, "Data" should be an empty byte array.

## Usage
1. Clone or download this repository
2. Open the CustomProcessingSample solution in Microsoft Visual Studio
3. Ensure you have the WindwardCustomProcessing.dll file from your .NET RESTful Engine (located in the bin folder)
4. Place the WindwardCustomProcessing.dll in the sample project and add a reference to it in Visual Studio
   1. We have a WindwardCustomProcessing.dll provided by default in the bin/Debug folder for your convinience
5. Build the solution
6. After building the solution navigate to the "obj/Debug" directory for the project, there you will find the "CustomProcessingSample.dll" file. Move this file to your RESTful Engine bin folder:
```
C:\inetpub\wwwroot\RESTfulEngine\bin
```
7. Copy the path to the file
8. Next we will add the following to our web.config file for the RESTful engine:
```
<appSettings>
   <add key="postProcessor" value="C:\inetpub\wwwroot\RESTfulEngine\bin\CustomProcessingSample.dll!CustomProcessingSample.CustomProcessor" />
</appSetings>
```
9. Now, start up your RESTful engine and run a report. When you get the report the "Data" will be empty

## Documentation
You can view a full tutorial on how to accomplish this in our documentation [here](https://fluent.apryse.com/documentation/engine-guide/Fluent%20RESTful%20Engines/NetRESTCustomProc).
