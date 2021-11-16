using Microsoft.Azure.Functions.Worker.Converters;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CustomConverterSampleApp.Converters
{
    public class ProductConverter : IInputConverter
    {
        private readonly ILogger<CustomerConverter> _logger;

        public ProductConverter(ILogger<CustomerConverter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
        {
            if (context.TargetType != typeof(Product))
            {
                return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
            }

            if (!context.FunctionContext.BindingContext.BindingData.TryGetValue("productId", out var prodIdObj))
            {
                return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
            }

            _logger.LogInformation("Populating Product information");

            var result = new Product { Name = "PROD-" + prodIdObj.ToString() };
            var conversionResult = ConversionResult.Success(result);

            return new ValueTask<ConversionResult>(conversionResult);
        }
    }

}
