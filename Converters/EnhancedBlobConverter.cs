using Microsoft.Azure.Functions.Worker.Converters;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CustomConverterSampleApp.Converters
{
    internal class EnhancedBlobConverter : IInputConverter
    {
        private readonly ILogger<EnhancedBlobConverter> _logger;
        private readonly IMyBlobService _myBlobService;
        public EnhancedBlobConverter(ILogger<EnhancedBlobConverter> logger, IMyBlobService myBlobService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _myBlobService = myBlobService ?? throw new ArgumentNullException(nameof(myBlobService));
        }

        public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
        {
            if (context.TargetType != typeof(CloudBlob))
            {
                return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
            }

            _logger.LogInformation("Populating CloudBlob information");

            context.FunctionContext.BindingContext.BindingData.TryGetValue("Uri", out var blobUrlObj);

            try
            {
                // URI has escaped double quotes.
                var cloudBlob = _myBlobService.GetCloudBlob(new Uri(blobUrlObj.ToString().Replace("\"", "")));

                return new ValueTask<ConversionResult>(ConversionResult.Success(cloudBlob));
            }
            catch (Exception ex)
            {
                return new ValueTask<ConversionResult>(ConversionResult.Failed(ex));
            }
        }
    }
}
