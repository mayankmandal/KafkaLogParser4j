using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

namespace KafkaLogParser4j
{
    public class Program
    {
        public static void Main()
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder();
            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = "KafkaLogParser4j Service";
            });

            // Register EventLogLoggerProvider options
            LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);

            builder.Services.AddSingleton<KafkaServers>();

            // Add IConfiguration
            builder.Services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build());

            builder.Services.AddHostedService<WindowsBackgroundService>();

            IHost host = builder.Build();
            host.Run();
        }
    }
}
