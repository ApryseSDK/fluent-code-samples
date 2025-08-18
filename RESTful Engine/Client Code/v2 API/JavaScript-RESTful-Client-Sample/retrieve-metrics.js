import pkg from 'windwardrestapi';
import fs from "fs";
const {WindwardClient, Template, Xml_10DataSource, OutputFormatEnum} = pkg;

const config = JSON.parse(fs.readFileSync('./config.json'));

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
};

async function main()
{
    /**
     * Step 1: INITIALIZE THE RESTFUL CLIENT AND SET UP THE TEMPLATE AND DATA SOURCE
     * --------------------------------------------------------------------------------------------
     */
    console.log("Initializing Fluent RESTful Client...");

    // Create a new instance of the client using the RESTful Engine URL provided in the config file
    let client = new WindwardClient.WindwardClient(config['restful-engine-url']);

    // Pass the license to the client.  The license should be specified in the config file
    // If you don't have a license key, you can leave it blank and it will produce output with a watermark.
    client.license = config['license'];
    
    // Display the version info for the restful engine
    let version = await client.getVersionInfo();
    console.log("Fluent RESTFul Engine Version: ", version);

    // The datasource document I am sending to the engine to be processed
    const dataSourcePath = './files/InvestmentFactSheet.xml';

    // Create the object and pass in the datasource name, and the data file path.
    let dataSource = new Xml_10DataSource('InvestmentFactSheet', undefined, dataSourcePath, undefined);
 
    //The template file I wish to process.
    const templatePath = './files/InvestmentFactSheet.docx';

    //Create a new template object (to see all the input params check the documentation)
    let template = new Template(OutputFormatEnum.PDF, [dataSource], undefined, templatePath,
        undefined, undefined, undefined, undefined, undefined, undefined,
        undefined, undefined, undefined, undefined, undefined, undefined);

    /**
     * Step 2: REQUEST THE TEMPLATE METRICS
     * --------------------------------------------------------------------------------------------
     */
    console.log("Requesting Template Metrics...");

    // Request metrics from the engine for our template
    let metrics = await client.postMetrics(template);
    
    //check postMetrics status and wait if not ready.
    while(true) {
        await sleep(1000);
        let status = await client.getMetricsStatus(metrics.Guid);
        if (status == 302) {
            // The metrics request is complete, we can now proceed to retrieve the template metrics
            break;
        }
        else if (status == 201 || status == 202 || status == 404) {
            // The metrics request is still in progress, continue waiting
            continue;
        }
        else {
            // Potentially have an error, proceed to retrieve the metrics to get error details
            console.error("Error retrieving metrics. Status code: ", status);
            break;
        }
    }

    /**
     * Step 3: RETRIEVE THE METRICS
     * --------------------------------------------------------------------------------------------
     */
    console.log("Retrieving Metrics...");

    // Retrieve the template metrics
    let templateMetrics = await client.getMetrics(metrics.Guid);

    // Write the template metrics to a file
    fs.writeFile("./files/metrics.json", JSON.stringify(templateMetrics, null, 2), function(err){});
    console.log("Template metrics saved to /files/metrics.json");

    // Delete the metrics from the engine
    await client.deeleteMetrics(templateMetrics.Guid);
}

// Execute the main() method
main();