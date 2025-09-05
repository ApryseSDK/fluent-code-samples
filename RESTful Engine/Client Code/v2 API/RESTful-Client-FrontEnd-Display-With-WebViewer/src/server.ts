import express from 'express';
import path from 'path';
import { readFileSync } from 'fs';
import { Buffer } from 'buffer';

// Assuming the Windward types are available from an installed package
import { WindwardClient, Xml_10DataSource, Template, OutputFormatEnum } from 'windwardrestapi';

// Define a helper for sleeping
const sleep = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));

interface AppConfig {
    'restful-engine-url': string;
    'fluent-license': string;
    'webviewer-license': string;
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

// --- DOCUMENT GENERATION LOGIC ---
async function generateDocument(): Promise<Buffer> {
    console.log("Initializing Fluent RESTful Client...");

    // Initialize the Fluent RESTful Client
    const client = new WindwardClient.WindwardClient(config['restful-engine-url']);
    client.license = config['fluent-license'];

    // Load up the template and data source
    const dataSourcePath = path.join(filesDir, 'InvestmentFactSheet.xml');
    const dataSource = new Xml_10DataSource('InvestmentFactSheet', undefined, dataSourcePath, undefined);

    const templatePath = path.join(filesDir, 'InvestmentFactSheet.docx');
    const template = new Template(OutputFormatEnum.PDF, [dataSource], undefined, templatePath);

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
    res.json({ webviewerLicense: config['webviewer-license'] });
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
});
