using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using KafkaClassLibrary;

namespace KafkaLogParser4j
{
    public sealed class WindowsBackgroundService : BackgroundService
    {
        private readonly KafkaServers _kafkaServers;
        private readonly ILogger<WindowsBackgroundService> _logger;
        private readonly IConfiguration _configuration;

        public WindowsBackgroundService(KafkaServers kafkaServers, IConfiguration configuration, ILogger<WindowsBackgroundService> logger)
        {
            _kafkaServers = kafkaServers;
            _configuration = configuration;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Semaphore semaphoreProducer = Semaphore.OpenExisting(SharedConstants.AppMutexNameProducer);

                _logger.LogInformation("Initiating Zookeeper and Kafka Brokers Servers Methods...");
                await _kafkaServers.KafkaServersMain(stoppingToken);

                semaphoreProducer.Release();
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Operation canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                // Environment.Exit(1);
            }
        }
    }
}
