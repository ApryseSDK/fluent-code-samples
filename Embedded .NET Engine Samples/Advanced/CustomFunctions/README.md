# Embedded .NET Engine Custom Functions Sample

## Overview
This sample project demonstrates how to create custom functions in the .NET engine and how to reference them so they can be utilized when generating output.

Custom Functions are referenced by tags in a Fluent Template and when the engine generates output, it will look to the custom function implementation defined in order to create an output value.

For this project, we have a template that references a custom function called *CUSTOMFUNCTION()* which will output a string specified by our custom function.

## Running the Project

> This project is setup  where you only need to click the *Start* button and it should all work without any additional changes
1. Open the project in Visual STudio
2. Insert your license in the App.config file in the FluentCustomFunctionsTest project
    1. If you don't have a license you can leave it blank and a watermark will be present in output
3. Set the *function.files* property in the App.config file in the FluentCustomFunctionsTest project to the path to the custom function dll (FluentCustomFunctions.dll)
4. Run the sample (FluentCustomFunctionTest) with the *Start* button


## Modifying the Custom Function
To modify the custom function:
1. Open the FluentCustomFunctions file in the *FluentCustomFunctions* project.
2. Modify the returned value in the `public static string CUSTOMFUNCTION()` method
3. Rebuild the **FluentCustomFunctions** project
4. Run the sample (FluentCustomFunctionTest) with the *Start* button

## Additional Setup Information
This section provides some more info about how this project is setup.

This solution contains 2 projects, **FluentCustomFunctions** which contains our custom functions implementation and another project **FluentCustomFunctionsTest** which generates a document using our custom function.

The **FluentCustomFunctions** project produces a dll to the *FluentCustomFunctions/bin/Debug* directory.
This dll is then directly referenced by the **FluentCustomFunctionsTest** project.

Using the *function.files* property in the App.config file then tells the Fluent engine process to look for our custom functions implementation and use that during the document generation process.

More information about setting up the custom functions can be found in the *More Information* section below.

## More Information
More information about how to use and setup custom functions with your application can be found [here](https://fluent.apryse.com/documentation/engine-guide/NET%20Engine/CustomFunctionNet).