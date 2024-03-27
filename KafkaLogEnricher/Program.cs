using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

namespace KafkaLogEnricher
{
    public class Program
    {
        private const string SingleInstanceMutex = "KafkaLogEnricherSingleMutex";
        public static void Main()
        {
            try
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
                            options.ServiceName = "KafkaLogEnricher Service";
                        });

                        // Register EventLogLoggerProvider options
                        LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);

                        builder.Services.AddSingleton<KafkaLogEnricher>();

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
            catch (Exception ex)
            {
                Console.WriteLine($"Issue Observed : {ex.Message.ToString()}");
            }
        }
    }
}
