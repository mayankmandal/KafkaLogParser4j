using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KafkaClassLibrary
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
                services.AddHostedService<KafkaServers>();
                services.AddSingleton<IConfiguration>(hostContext.Configuration);
            });
    }
}
