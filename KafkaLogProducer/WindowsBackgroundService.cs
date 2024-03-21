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
                _logger.LogInformation("Initiating Producer Methods...");
                await _kafkaLogProducer.ProducerMain(stoppingToken);
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
