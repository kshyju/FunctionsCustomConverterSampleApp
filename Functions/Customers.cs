using System.Net;
using CustomConverterSampleApp.Converters;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Converters;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CustomConverterSampleApp
{
    public class Customers
    {
        private readonly ILogger _logger;

        public Customers(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Customers>();
        }

        [Function("Customers")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req, Customer customer)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            if (customer is not null)
                response.WriteString($"Customer: Id:{customer.Id}, Name: {customer.Name}");
            else
                response.WriteString($"Could not populate customer information. Pass a 'customer' query parameter with int value. Ex: ?customer=2021");

            return response;
        }
    }

    [InputConverter(typeof(CustomerConverter))]
    public record Customer(int Id, string Name);
}
