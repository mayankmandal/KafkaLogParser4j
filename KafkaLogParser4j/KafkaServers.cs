﻿using KafkaClassLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace KafkaLogParser4j
{
    public sealed class KafkaServers
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public KafkaServers(IConfiguration configuration, ILogger<KafkaServers> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task KafkaServersMain(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Initiated ZooKeeper : {_configuration["KafkaConfigs:ZookeeperServer:ZooKeeperName"]}");

            // Start Zookeeper server
            ExecuteCommandInBackground(
                _configuration["KafkaConfigs:ZookeeperServer:ZooKeeperName"],
                _configuration["KafkaConfigs:ZookeeperServer:ZooKeeperBatPath"],
                _configuration["KafkaConfigs:ZookeeperServer:ZooKeeperConfigPath"],
                cancellationToken);

            // Delay for 45 seconds
            Task.Delay(TimeSpan.FromSeconds(60), cancellationToken).Wait(cancellationToken);

            // Check if cancellation was requested
            if (cancellationToken.IsCancellationRequested)
            {
                // Handle cancellation if needed
                return;
            }

            _logger.LogInformation($"Initiated Kafka Broker 1 : {_configuration["KafkaConfigs:KafkaClients:KafkaBrokerName1"]}");

            // Start Kafka server 0
            ExecuteCommandInBackground(
                _configuration["KafkaConfigs:KafkaClients:KafkaBrokerName1"],
                _configuration["KafkaConfigs:KafkaClients:KafkaBrokerBatPath1"],
                _configuration["KafkaConfigs:KafkaClients:KafkaBrokerConfigPath1"],
                cancellationToken);

            _logger.LogInformation($"Initiated Kafka Broker 2 : {_configuration["KafkaConfigs:KafkaClients:KafkaBrokerName2"]}");

            // Start Kafka server 1
            ExecuteCommandInBackground(
                _configuration["KafkaConfigs:KafkaClients:KafkaBrokerName2"],
                _configuration["KafkaConfigs:KafkaClients:KafkaBrokerBatPath2"],
                _configuration["KafkaConfigs:KafkaClients:KafkaBrokerConfigPath2"],
                cancellationToken);

            _logger.LogInformation($"Initiated Kafka Broker 3 : {_configuration["KafkaConfigs:KafkaClients:KafkaBrokerName3"]}");

            // Start Kafka server 2
            ExecuteCommandInBackground(
                _configuration["KafkaConfigs:KafkaClients:KafkaBrokerName3"],
                _configuration["KafkaConfigs:KafkaClients:KafkaBrokerBatPath3"],
                _configuration["KafkaConfigs:KafkaClients:KafkaBrokerConfigPath3"],
                cancellationToken);

            // Delay for 30 seconds
            Task.Delay(TimeSpan.FromSeconds(30), cancellationToken).Wait(cancellationToken);
        }

        private static void ExecuteCommandInBackground(string processName, string executablePath, string arguments, CancellationToken cancellationToken)
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
            cancellationToken.Register(() => process.Kill());
        }
    }
}
