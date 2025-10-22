import express from 'express';
import path from 'path';
import { readFileSync } from 'fs';
import { Buffer } from 'buffer';
import { getContentAsBase64 } from './utils';

import { 
    WindwardClient, 
    Xml_10DataSource, 
    Xml_20DataSource,
    JsonDataSource, 
    SqlDataSource, 
    Template, 
    OutputFormatEnum
} from 'windwardrestapi';

const DEFAULT_RESTFUL_CONFIG_VALUE = "[YOUR_RESTFUL_ENGINE_URL]";

// Define a helper for sleeping
const sleep = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));

interface AppConfig {
    'restful-engine-url': string;
    'fluent-license': string;
    'webviewer-license': string;
    'template-connection-string': string;
    'output-format': string;
    'datasources': Array<{
        'name': string;
        'type': string;
        'connection-string': string;
    }>;
    'input-parameters': Array<{
        'name': string;
        'type': string;
        'value': string;
    }>;
}

const app = express();
const PORT = 3000;

// Define robust paths
const projectRoot = path.join(__dirname, '..'); 

// Load configuration from config.json
const configPath = path.join(projectRoot, 'config.json');
const config: AppConfig = JSON.parse(readFileSync(configPath, 'utf-8'));

// Use robust paths for all file I/O
const filesDir = path.join(projectRoot, 'files');

// --- TEMPLATE, DATA SOURCE AND INPUT PARAMETER INITIALIZATION ---
async function intializeTemplate(): Promise<any> {
    const templateData = await getContentAsBase64(config['template-connection-string']);
    const outputFormatStr = config['output-format'] as string;
    let outputFormat;
    switch(outputFormatStr.toLowerCase()) {
        case 'docx':
            outputFormat = OutputFormatEnum.DOCX;
            break;
        case 'xlsx':
            outputFormat = OutputFormatEnum.XLSX;
            break;
        case 'pptx':
            outputFormat = OutputFormatEnum.PPTX;
            break;
        case 'html':
            outputFormat = OutputFormatEnum.HTML;
            break;
        case 'pdf':
        default:
            outputFormat = OutputFormatEnum.PDF;
            break;
    }

    return new Template(outputFormat, await intializeDataSource(), undefined, templateData, undefined, undefined, undefined, initializeInputParameters());
}

async function intializeDataSource(): Promise<any[]> {
    const dataSourcesConfig = config['datasources'];
    const dataSources: any[] = [];
    for (const dsConfig of dataSourcesConfig) {
        let dsData = dsConfig['connection-string'];
        let dataSource;
        switch (dsConfig['type']) {
            case 'xml':
                dsData = await getContentAsBase64(dsConfig['connection-string']);
                dataSource = new Xml_10DataSource(dsConfig['name'], undefined, dsData, undefined);
                break;
            case 'xml2':
                dsData = await getContentAsBase64(dsConfig['connection-string']);
                dataSource = new Xml_20DataSource(dsConfig['name'], undefined, dsData, undefined);
                break;
            case 'json':
                dsData = await getContentAsBase64(dsConfig['connection-string']);
                dataSource = new JsonDataSource(dsConfig['name'], undefined, dsData, undefined);
                break;
            case 'sql':
                dataSource = new SqlDataSource(dsConfig['name'], "com.microsoft.sqlserver.jdbc.SQLServerDriver", dsData, undefined);
                break;
            default:
                break;
        }
        
        if (dataSource) {
            dataSources.push(dataSource);
        }
    }
    return dataSources;
}

function initializeInputParameters(): any[] {
    const inputParamsConfig = config['input-parameters'];
    const parameters: any[] = [];
    inputParamsConfig.forEach(paramConfig => {
        let val: any = {
            ParamType: paramConfig['type'],
            RawValue: paramConfig['value']
        };

        let param: any = {
            Name: paramConfig['name'],
            WrappedValue: val
        }
        parameters.push(param);
    });

    return parameters;
}

// --- DOCUMENT GENERATION LOGIC ---
async function generateDocument(): Promise<Buffer> {
    console.log("Initializing Fluent RESTful Client...");

    // Initialize the Fluent RESTful Client
    const client = new WindwardClient.WindwardClient(config['restful-engine-url'], config['fluent-license']);

    // Initialize the template and data sources
    const template = await intializeTemplate();

    // Generate the document
    console.log("Generating Document...");
    const document = await client.postDocument(template);

    // Wait for report generation to complete (or error out)
    while (true) {
        await sleep(1000);
        const status: number = await client.getDocumentStatus(document.Guid);
        if (status === 302) {
            console.log("Document generation complete.");
            break;
        } else if ([201, 202, 404].includes(status)) {
            console.log(`Processing... (status: ${status})`);
        } else {
            throw new Error(`Error generating document. Status code: ${status}`);
        }
    }

    // Get the document once its done
    console.log("Retrieving Document...");
    const generatedDocument = await client.getDocument(document.Guid);

    // Clean up the document on the RESTful Engine server
    await client.deleteDocument(generatedDocument.Guid);

    // Return the generated document data as a Buffer
    return Buffer.from(generatedDocument.Data, "base64");
}

// --- MIDDLEWARE SETUP ---
app.use(express.static(path.join(projectRoot, 'public')));
app.use('/files', express.static(filesDir));

// --- API ROUTES ---
app.get('/config', (req, res) => {
    res.json(
        {
            webviewerLicense: config['webviewer-license'],
            templateConnectionString: config['template-connection-string'],
            outputFormat: config['output-format']
        }
    );
});

app.post('/generate-document', async (req, res) => {
    console.log("Received request to generate document.");
    try {
        const buffer = await generateDocument();
        res.setHeader('Content-Type', 'application/pdf');
        res.send(buffer);
    } catch (error) {
        console.error("Error during document generation:", error);
        res.status(500).json({ success: false, message: 'Failed to generate document.' });
    }
});

// --- MAIN ROUTE ---
app.get('*', (req, res) => {
    res.sendFile(path.join(projectRoot, 'public/index.html'));
});

// --- SERVER START ---
app.listen(PORT, () => {
    console.log(`Server is running at http://localhost:${PORT}`);

    if (config['restful-engine-url'] === '' || config['restful-engine-url'] === DEFAULT_RESTFUL_CONFIG_VALUE) {
        throw new Error("Please update the 'restful-engine-url' in config.json with your RESTful Engine URL.");
    }
});

