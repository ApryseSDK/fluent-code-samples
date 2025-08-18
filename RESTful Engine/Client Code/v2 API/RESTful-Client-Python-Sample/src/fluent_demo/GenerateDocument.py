from .config import load_config, project_root, require_file
from windwardrestapi.Model import Template, Xml_10DataSource
from windwardrestapi.Api import WindwardClient as client
from pathlib import Path
from requests import HTTPError
import base64
import zipfile
import time


def main():
    #Read in RESTful server URL and license key from Config file
    cfg = load_config()

    fluent = cfg["fluent"]
    paths = cfg["paths"]

    base_url = fluent["base_url"]
    license_key = fluent["license_key"]

    template_path = require_file(project_root() / paths["template_file"], "template file")
    data_path = require_file(project_root() / paths["data_file"], "XML data file")

    # -------------- DOCUMENT WORK FLOW -------------- #

    # Initialize client and set license
    fluent_client = client.WindwardClient(base_url)
    fluent_client.licenseKey = license_key

    print(fluent_client.getVersion().toString())

    # Build the request body
    xml_ds = Xml_10DataSource.Xml_10DataSource(name="MANF_DATA_2009", data=data_path)
    template = Template.Template(
        data=template_path,
        outputFormat=Template.outputFormatEnum.DOCX,
        datasources=[xml_ds],
    )

    # POST, check status for completion
    try:
        doc = fluent_client.postDocument(template)
    except HTTPError as e:
        resp = getattr(e, "response", None)
        print("POST /v2/document failed")
        print("Status:", getattr(resp, "status_code", None))
        print("Body:", resp.text if resp is not None else "(no body)")
        raise

    print("GUID:", doc.guid)


    #OPTIONAL: Add timeout counter incase something goes wrong
    MAX_WAIT_SECONDS = 60
    start = time.time()
    deadline = start + MAX_WAIT_SECONDS
    #wait for document to generate, and check status for completion or error
    while True:
        #If we are passed the alloted time, end execution
        if time.time() >= deadline:
            print("Timeout waiting for document to be ready.")
            break

        # Sleep like the JS example: await sleep(1000)
        remaining = max(0, deadline - time.time())
        time.sleep(min(1, remaining))

        status = fluent_client.getDocumentStatus(doc.guid)

        if status == 302:
            # The document generation is complete, we can now proceed to retrieve the generated document
            print("Ready:", status)
            break
        elif status in (201, 202, 404):
            # The document generation is still in progress, continue waiting
            continue
        else:
            # Potentially have an error
            raise Exception("Error generating document. Status code:", status)
            break

    # Save output file to output directory
    out_dir = project_root() / "output"
    out_dir.mkdir(parents=True, exist_ok=True)
    file_path = out_dir / "output.docx"

    raw_bytes = fluent_client.getDocumentFile(doc.guid)
    with open(file_path, "wb") as fh:
        fh.write(raw_bytes)

    assert zipfile.is_zipfile(file_path), f"Not a valid DOCX/ZIP: {file_path}"
    with zipfile.ZipFile(file_path) as zf:
        assert zf.read("word/document.xml") is not None


if __name__ == "__main__":
    main()
