using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KafkaLogEnricher
{
    public class Program
    {
        public static void Main()
        {
            CreateHostBuilder().Build().Run();
        }
        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<KafkaLogEnricher>();
                services.AddSingleton<IConfiguration>(hostContext.Configuration);
            });

    }
}
