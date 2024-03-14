using System.Diagnostics;
using System.ServiceProcess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KafkaLogParser4j
{
    public class KafkaLogParser4jService : BackgroundService
    {
        private readonly ILogger<KafkaLogParser4jService> _logger;
        public KafkaLogParser4jService(ILogger<KafkaLogParser4jService> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("KafkaLogParser4jService is starting.");

            // Add your start-up logic here

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("KafkaLogParser4jService is stopping.");

            // Add your stop logic here

            return base.StopAsync(cancellationToken);
        }
        public override void Dispose()
        {
            _logger.LogInformation("KafkaLogParser4jService is disposing.");

            // Add your disposal logic here

            base.Dispose();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("KafkaLogParser4jService is running.");

            // Start other main methods as background tasks here
            var task1 = Task.Run(() => OtherMain1(), stoppingToken);
            //var task2 = Task.Run(() => OtherMain2(), stoppingToken);
            //var task3 = Task.Run(() => OtherMain3(), stoppingToken);
            //var task4 = Task.Run(() => OtherMain4(), stoppingToken);

            // Wait for all tasks to complete
            await Task.WhenAll(task1);//, task2, task3, task4);
        }
        private void OtherMain1()
        {
            StartBackgroundProcess1("dotnet", @"C:\Users\MayankCoinStation\source\repos\KafkaLogParser4j\KafkaLogProducer\bin\Debug\net6.0\KafkaClassLibrary.dll", "KafkaClassLibrary.Program.Main");
        }
        private void StartBackgroundProcess1(string command, string filePath, string method)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = $"exec {filePath} --method {method}",
                UseShellExecute = false, // Don't use shell execution
                CreateNoWindow = true, // Don't create a visible window
            };

            Process process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            // Start the process
            process.Start();
        }
        private void OtherMain2()
        {
            KafkaLogProducer.Program.Main();
        }
        private void OtherMain3()
        {
            KafkaLogEnricher.Program.Main();
        }
        private void OtherMain4()
        {
            KafkaLogConsumer.Program.Main();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<KafkaLogParser4jService>();
                })
                .Build()
                .Run();
        }
    }
}
