using KafkaClassLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using System.Diagnostics;

namespace KafkaLogParser4j
{
    public class Program
    {
        private const string SingleInstanceMutex = "KafkaLogParser4jSingleMutex";
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
                        // Register the ProcessExit Event
                        AppDomain.CurrentDomain.ProcessExit += (s, e) => OnProcessExit();

                        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
                        builder.Services.AddWindowsService(options =>
                        {
                            options.ServiceName = "KafkaLogParser4j Service";
                        });

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
            catch (Exception ex)
            {
                Console.WriteLine($"Issue Observed : {ex.Message.ToString()}");
            }
        }
        private static void OnProcessExit()
        {
            // Perform CleanUp Activity
            Console.WriteLine("Shutting down Zookeeper servers and Kafka brokers...");

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

            // Stop Kafka servers
            ExecuteCommandInBackground(
                configuration["KafkaConfigs:KafkaClients:KafkaBrokerServersStopName"],
                configuration["KafkaConfigs:KafkaClients:KafkaBrokerServersStop"],
                SharedConstants.MagicString);

            // Stop Zookeeper server
            ExecuteCommandInBackground(
                configuration["KafkaConfigs:ZookeeperServer:ZooKeeperServerStopName"],
                configuration["KafkaConfigs:ZookeeperServer:ZooKeeperServerStop"],
                SharedConstants.MagicString);
        }
        private static void ExecuteCommandInBackground(string processName, string executablePath, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            Process process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
            process.Start();
            // Set the process name to make it identifiable in the Task Manager
            process.StartInfo.FileName = processName;
        }
    }
}
