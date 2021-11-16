using Microsoft.Azure.Functions.Worker.Converters;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CustomConverterSampleApp.Converters
{
    public class CustomerConverter : IInputConverter
    {
        private readonly ILogger<CustomerConverter> _logger;

        public CustomerConverter(ILogger<CustomerConverter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
        {
            if (context.TargetType != typeof(Customer))
            {
                return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
            }

            if (context.Source is string customerIdStr && int.TryParse(customerIdStr, out var customerId))
            {
                _logger.LogInformation($"Populating customer profile for {customerId}");

                var result = new Customer(customerId, "Customer-" + customerId);

                return new ValueTask<ConversionResult>(ConversionResult.Success(result));
            }

            return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
        }
    }
}
