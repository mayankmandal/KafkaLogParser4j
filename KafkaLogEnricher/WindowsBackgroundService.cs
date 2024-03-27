using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using KafkaClassLibrary;

namespace KafkaLogEnricher
{
    public sealed class WindowsBackgroundService : BackgroundService
    {
        private readonly KafkaLogEnricher _kafkaLogEnricher;
        private readonly ILogger<WindowsBackgroundService> _logger;
        private readonly IConfiguration _configuration;

        public WindowsBackgroundService(KafkaLogEnricher kafkaLogEnricher, IConfiguration configuration, ILogger<WindowsBackgroundService> logger)
        {
            _kafkaLogEnricher = kafkaLogEnricher;
            _configuration = configuration;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(2));

                _logger.LogInformation("Waiting for KafkaLogProducer Service to Start...");

                Semaphore semaphoreEnricher = Semaphore.OpenExisting(SharedConstants.AppMutexNameEnricher);
                semaphoreEnricher.WaitOne();

                _logger.LogInformation("Initiating Enricher Methods...");
                Semaphore semaphoreConsumer = Semaphore.OpenExisting(SharedConstants.AppMutexNameConsumer);

                _kafkaLogEnricher.EnricherMain(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(10));

                _logger.LogInformation("KafkaLogEnricher Service Started. Signaling next service to start.");

                // Signal the next application to Start
                semaphoreConsumer.Release();
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Operation canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                Environment.Exit(1);
            }
        }
    }
}
