using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

namespace KafkaLogProducer
{
    public class Program
    {
        public static void Main()
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder();
            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = "KafkaLogProducer Service";
            });

            // Register EventLogLoggerProvider options
            LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);
            
            builder.Services.AddSingleton<KafkaLogProducer>();

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
