
using System.Threading.Tasks;
using WindwardCustomProcessing;

namespace CustomProcessingSample
{
    public class CustomProcessor : IDocumentPostProcessor
    {
        //This is the only method that needs to be defined in the IDocumentPostProcessor.
        //You can implement any other helper methods, but this is the method the RESTful engine will call.
        public Task<CustomDocument> Process(CustomDocument document)
        {
            //Setting the document data to an empty byte array
            document.Data = new byte[0];
            return Task.FromResult(document);
        }
    }
}
