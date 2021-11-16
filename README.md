# FunctionsCustomConverterSampleApp
Sample app demonstrating usage of custom converters.

The `EnhancedBlobListener` blob trigger function is using a custom converter (`EnhancedBlobConverter`) to populate a fully loaded `CloudBlob` object.

    [Function("EnhancedBlobListener")]
    public void Run(
        [BlobTrigger("samples-workitems/{name}")] string myBlob, string name, 
        [InputConverter(typeof(EnhancedBlobConverter))]CloudBlob cloudBlob)
    {
        _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} from container:{cloudBlob.Container.Name}");
    }
