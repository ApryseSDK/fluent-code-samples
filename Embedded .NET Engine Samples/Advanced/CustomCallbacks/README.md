# Embedded .NET Engine Custom Callbacks Sample

## Overview
This sample project demonstrates how to create custom callbacks in the java engine and how to reference them so they can be utilized when generating output.

Custom Callbacks are used when evaluating tag's select statements and allow the user to modify the select or perform actions via code based on the selects content.
In our example, we have 2 different checks in our callback.
1. If the select contains a specific string, then we return a new select with a modified value.
2. If the select contains a string we want to prohibit (i.e. customer's address), then we throw an exception

These are just a couple of simple examples to demonstrate how powerful custom callbacks can be.

## Running the Project

> This project is setup  where you only need to click the *Start* button and it should all work without any additional changes
1. Open the project in Visual Studio
2. Insert your license in the App.config file in the FluentCustomCallbacksTest project
    1. If you don't have a license you can leave it blank and a watermark will be present in output
3. Set the *callback.class* property in the App.config file in the FluentCustomCallbacksTest project to the path to the custom function dll (FluentCustomCallbacks.dll)
    1. **This NEEDS to be an absolute path to the dll due to a .NET Framwork security restriction on this property name specifically.**
4. Set *FluentCustomCallbacksTest* as the startup project by right clicking on the *FluentCustomCallbacksTest* and selecting "Set as Startup Project".
5. Run the sample (FluentCustomCallbacksTest) with the *Start* button


## Modifying the Custom Function
To modify the custom function:
1. Open the FluentCustomCallbacks file in the *FluentCustomCallbacks* project.
2. Modify the returned value in the `public static string CUSTOMFUNCTION()` method
3. Rebuild the **FluentCustomCallbacks** project
4. Run the sample (FluentCustomCallbacksTest) with the *Start* button

## Additional Setup Information
This section provides some more info about how this project is setup.

This project contains 2 modules, **CustomCallbacksExample** which contains our custom callback implementation and another module **CustomCallbacksTest** which generates a document to test our custom callbacks.

The **CustomCallbacksExample** project produces an artifact to the *CustomCallbacksExample/out/artifacts/FluentCustomCallbacks* directory.
This artifact (jar file) is then directly referenced by the **CustomCallbacksTest** project.

Using the *callback.class* property in the WindwardReports.properties file then tells the Fluent engine process to look for our custom callbacks implementation and use that during the document generation process.