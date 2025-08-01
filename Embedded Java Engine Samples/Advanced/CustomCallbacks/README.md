# Embedded Java Engine Custom Callbacks Sample

## Overview
This sample project demonstrates how to create custom callbacks in the java engine and how to reference them so they can be utilized when generating output.

Custom Callbacks are used when evaluating tag's select statements and allow the user to modify the select or perform actions via code based on the selects content.
In our example, we have 2 different checks in our callback.
1. If the select contains a specific string, then we return a new select with a modified value.
2. If the select contains a string we want to prohibit (i.e. customer's address), then we throw an exception

These are just a couple of simple examples to demonstrate how powerful custom callbacks can be.

## Running the Project

> This project is setup  where you only need to click the *Run* button and it should all work without any additional changes

1. Open the project in Intellij
2. Insert your license in the WindwardReports.properties file in the CustomCallbacksTest project
    1. If you don't have a license you can leave it blank and a watermark will be present in output
3. Set the *callback.class* property in WindwardReports.properties in the CustomCallbacksTest to the fully qualified name of the custom callback class being referenced
4. Run the sample (FluentCustomFunctionTest) with the *Run* button


## Modifying the Custom Callback
To modify the custom callback:
1. Open the FluentCustomCallbacks file in the *FluentCustomCallbacks* project.
2. Modify the code in the `approveSelect()` method
3. Rebuild the **CustomCallbacksExample** module
4. Run the sample file (FluentCustomCallbacksTest) with the *Run* button

## Additional Setup Information
This section provides some more info about how this project is setup.

This project contains 2 modules, **CustomCallbacksExample** which contains our custom callback implementation and another module **CustomCallbacksTest** which generates a document to test our custom callbacks.

The **CustomCallbacksExample** project produces an artifact to the *CustomCallbacksExample/out/artifacts/FluentCustomCallbacks* directory.
This artifact (jar file) is then directly referenced by the **CustomCallbacksTest** project.

Using the *callback.class* property in the WindwardReports.properties file then tells the Fluent engine process to look for our custom callbacks implementation and use that during the document generation process.