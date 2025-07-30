# Embedded Java Engine Custom Functions Sample

## Overview
This sample project demonstrates how to create custom functions in the java engine and how to reference them so they can be utilized when generating output.

Custom Functions are referenced by tags in a Fluent Template and when the engine generates output, it will look to the custom function implementation defined in order to create an output value.

For this project, we have a template that references a custom function called *CUSTOMFUNCTION()* which will output a string specified by our custom function.

## Running the Project

> This project is setup  where you only need to click the *Run* button and it should all work without any additional changes

1. Open the project in Intellij
2. Insert your license in the WindwardReports.properties file
    1. If you don't have a license you can leave it blank and a watermark will be present in output
3. Set the *function.files* property in WindwardReports.properties to the fully qualified name of the custom function file being referenced
4. Run the sample (FluentCustomFunctionTest) with the *Run* button


## Modifying the Custom Function
To modify the custom function:
1. Open the FluentCustomFunctions file in the *CustomFunctionsExample* project.
2. Modify the returned value in the `public static Object CUSTOMFUNCTION()` method
3. Rebuild the **CustomFunctionsExample** module
4. Run the sample (FluentCustomFunctionTest) with the *Run* button

## Additional Setup Information
This section provides some more info about how this project is setup.

This project contains 2 modules, **CustomFunctionsExample** which contains our custom functions implementation and another module **CustomFunctionsTest** which generates a document using our custom function.

The **CustomFunctionsExample** project produces an artifact to the *CustomFunctionsExample/out/artifacts/FluentCustomFunctions* directory.
This artifact (jar file) is then directly referenced by the **CustomFunctionsTest** project.

Using the *function.files* property in the WindwardReports.properties file then tells the Fluent engine process to look for our custom functions implementation and use that during the document generation process.

More information about setting up the custom functions can be found in the *More Information* section below.

## More Information
More information about how to use and setup custom functions with your application can be found [here](https://fluent.apryse.com/documentation/engine-guide/Java%20Engine/CustomFunctionJava).