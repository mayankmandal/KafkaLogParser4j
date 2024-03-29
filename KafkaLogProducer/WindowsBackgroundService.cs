using KafkaClassLibrary;
namespace KafkaLogProducer
{
    public sealed class WindowsBackgroundService : BackgroundService
    {
        private readonly KafkaLogProducer _kafkaLogProducer;
        private readonly ILogger<WindowsBackgroundService> _logger;
        private readonly IConfiguration _configuration;
        public WindowsBackgroundService(KafkaLogProducer kafkaLogProducer, IConfiguration configuration, ILogger<WindowsBackgroundService> logger)
        {
            _kafkaLogProducer = kafkaLogProducer;
            _configuration = configuration;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(2));

                _logger.LogInformation("Waiting for KafkaLogParser4j Service to Complete...");

                Semaphore semaphoreProducer = Semaphore.OpenExisting(SharedConstants.AppMutexNameProducer);
                semaphoreProducer.WaitOne();

                _logger.LogInformation("Initiating Producer Method...");
                Semaphore semaphoreEnricher = Semaphore.OpenExisting(SharedConstants.AppMutexNameEnricher);

                _kafkaLogProducer.ProducerMain(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(10));

                _logger.LogInformation("KafkaLogProducer Service Started. Signaling next service to start.");

                // Signal the next application to Start
                semaphoreEnricher.Release();
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
