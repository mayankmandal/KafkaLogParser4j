using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

namespace KafkaLogParser4j
{
    public class Program
    {
        private const string SingleInstanceMutex = "KafkaLogParser4jSingleMutex";
        public static void Main()
        {
            // Attempt to acquire the mutex
            using (var mutex = new Mutex(true, SingleInstanceMutex, out bool createdNew))
            {
                // If the mutex was successfully created, it means first instance
                if (createdNew)
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
                else
                {
                    Console.WriteLine("Another instance of the application is already running. Exiting...");
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }
        }
    }
}
