using CustomConverterSampleApp.Converters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CustomConverterSampleApp
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults((workerOptions) =>
                {
                    workerOptions.InputConverters.Register<ProductConverter>();
                })
                .ConfigureServices((services) =>
                {
                    services.AddSingleton<IMyBlobService, MyBlobService>();
                })
                .Build();

            host.Run();
        }
    }
}