//using System;
//using System.IO;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Extensions.Logging;

//namespace CustomConverterSampleApp.Functions
//{
//    public class BlobListener
//    {
//        private readonly ILogger _logger;

//        public BlobListener(ILoggerFactory loggerFactory)
//        {
//            _logger = loggerFactory.CreateLogger<BlobListener>();
//        }

//        [Function("BlobListener")]
//        public void Run([BlobTrigger("samples-workitems/{name}")] string myBlob, string name)
//        {
//            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {myBlob}");
//        }
//    }
//}
