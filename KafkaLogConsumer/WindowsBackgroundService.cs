using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using KafkaClassLibrary;
namespace KafkaLogConsumer
{
    public sealed class WindowsBackgroundService : BackgroundService
    {
        private readonly KafkaLogConsumer _kafkaLogConsumer;
        private readonly ILogger<WindowsBackgroundService> _logger;
        private readonly IConfiguration _configuration;
        private readonly CancellationTokenSource _cancellationTokenSource;
        public WindowsBackgroundService(KafkaLogConsumer kafkaLogConsumer, IConfiguration configuration, ILogger<WindowsBackgroundService> logger, CancellationTokenSource cancellationTokenSource)
        {
            _kafkaLogConsumer = kafkaLogConsumer;
            _configuration = configuration;
            _logger = logger;
            _cancellationTokenSource = cancellationTokenSource;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(2));

                _logger.LogInformation("Waiting for KafkaLogEnricher Service to Start...");

                Semaphore semaphoreConsumer = Semaphore.OpenExisting(SharedConstants.AppMutexNameConsumer);
                semaphoreConsumer.WaitOne();

                _logger.LogInformation("Initiating Consumer Methods...");
                await _kafkaLogConsumer.ConsumerMain(stoppingToken);
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
