using WindwardRestApi.src.Model;

namespace BatchProcessing
{
    internal class DocumentResult
    {
        public string JobName { get; set; }

        public Document Document { get; set; }

        public string JobId { get; set; }
    }
}
