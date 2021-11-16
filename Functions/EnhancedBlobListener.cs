using CustomConverterSampleApp.Converters;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Converters;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;

namespace CustomConverterSampleApp.Functions
{
    public class EnhancedBlobListener
    {
        private readonly ILogger _logger;

        public EnhancedBlobListener(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<EnhancedBlobListener>();
        }

        [Function("EnhancedBlobListener")]
        public void Run(
            [BlobTrigger("samples-workitems/{name}")] string myBlob, string name, 
            [InputConverter(typeof(EnhancedBlobConverter))]CloudBlob cloudBlob)
        {
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} from container:{cloudBlob.Container.Name}");
        }
    }
}
