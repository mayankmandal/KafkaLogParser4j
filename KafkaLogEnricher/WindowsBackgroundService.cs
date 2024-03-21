using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                _logger.LogInformation("Initiating Enricher Methods...");
                _kafkaLogEnricher.EnricherMain(stoppingToken);
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
