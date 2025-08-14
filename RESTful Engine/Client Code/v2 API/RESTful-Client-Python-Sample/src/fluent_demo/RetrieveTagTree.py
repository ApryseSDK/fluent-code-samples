from .config import load_config, project_root, require_file
from windwardrestapi.Model import Template, Xml_10DataSource
from windwardrestapi.Api import WindwardClient as client
from requests import HTTPError
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

     # -------------- TAGTREE WORK FLOW -------------- #

     # Initialize client and set license
    fluent_client = client.WindwardClient(base_url)
    fluent_client.licenseKey = license_key

    print(fluent_client.getVersion().toString())

    xml_ds = Xml_10DataSource.Xml_10DataSource(name="MANF_DATA_2009", data=data_path)
    tmpl = Template.Template(
        data=template_path,
        outputFormat=Template.outputFormatEnum.DOCX,
        datasources=[xml_ds],
    )

    try:
        #post tagtree
        tagtree_job = fluent_client.postTagTree(tmpl)
    except HTTPError as e:
        resp = getattr(e, "response", None)
        print("POST /v2/tagtree failed")
        print("Status:", getattr(resp, "status_code", None))
        print("Body:", resp.text if resp is not None else "(no body)")
        raise

    print("TagTree GUID:", tagtree_job.guid)

    #wait for completed status
    while True:
        status = fluent_client.getTagTreeStatus(tagtree_job.guid)
        if status == 302:
            print("TagTree ready:", status)
            break
        print("TagTree not ready:", status)
        time.sleep(1)
    #get tagtree
    tagtree = fluent_client.getTagTree(tagtree_job.guid)
    print("TAGTREE\n", tagtree.toDict())


if __name__ == "__main__":
    main()
