# Embedded Java Engine Semaphore Sample

## Overview
This sample project demonstrates generating multiple documents while limiting the number of threads processing at one time using a semaphore.  This is useful for when your license has a thread limit and you don't want to exceed the limit.

## Running the Project
1. Open the project in Intellij
2. Insert your license in the WindwardReports.properties file
   1. If you don't have a license you can leave it blank and a watermark will be present in output
3. Set the *numThreads* property in WindardReports.properties to specify a thread limit (Default is 2)
4. Run the sample with the *Run* button