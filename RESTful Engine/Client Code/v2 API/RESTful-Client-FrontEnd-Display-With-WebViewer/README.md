# Fluent RESTful Client v2 With Webviewer Sample

## Overview
This is a sample application that uses the Fluent JavaScript RESTful Client to generate a document and displays the result with Apryse WebViewer.  This application, when started will allow you to navigate to the webpage, click the "Generate Document" button and will then show the output in the WebViewer window.  The WebViewer window will initially be populated with the sample template.

There are 3 main files to worry about:
1. server.ts
    * This file is the Node.js server file that also contains the document generation logic
2. index.html
    * This file contains the code for the webpage as well as initializing and using Apryse WebViewer
3. config.json
    * This file contains the config options to specify your license keys and RESTful Engine instance to be used by the application

## Requirements
* Make sure you have Node.js installed on your machine (which includes npm).
* You need a running instance of the Fluent RESTful Engine.
* (Optional) WebViewer License Key. To get a trial key you can go [here](https://docs.apryse.com/web/guides/get-started/trial-key).
    * If you don't specify a license, a watermark will be present on output, but it is still fully usable
* (Optional) Fluent Engine Key. To get a trial key you can go [here](https://apryse.com/form/fluent-trial).
    * If you don't specify a license, a watermark will be present on output, but it is still fully usable

## Usage
1. Open the `config.json` file and insert your RESTful Engine URL **(This is required)**
    * You can insert your Fluent and WebViewer license keys as well but this isn't required to use the sample.
2. Open a terminal and navigate to the root of this project
3. Run the command `npm install` to install the required packages
4. Run the command `npm run build` to build the project
5. Run the command `npm start` to start the project.  This should start the server at `http://localhost:3000`.
6. Open your browser and navigate to `http://localhost:3000` to start using the sample app.


## Additional Information
* More information about the Fluent Client API can be found [here](https://www.npmjs.com/package/windwardrestapi).
* More information about using WebViewer can be found [here](https://docs.apryse.com/web/guides/get-started).