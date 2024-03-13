using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaClassLibrary
{
    public class KafkaServers : BackgroundService
    {
        private readonly IConfiguration _configuration;
        public KafkaServers(IConfiguration configuration)
        {
           _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // Start Kafka Servers
            await KafkaServersMain(cancellationToken);
        }
        public async Task KafkaServersMain(CancellationToken cancellationToken)
        {
            // Start Zookeeper server
            ExecuteCommandInBackground(_configuration["KafkaConfigs:ZookeeperServer:ZooKeeperName"], @_configuration["KafkaConfigs:ZookeeperServer:ZooKeeperBatPath"], @_configuration["KafkaConfigs:ZookeeperServer:ZooKeeperConfigPath"], cancellationToken);
            await Task.Delay(TimeSpan.FromSeconds(45), cancellationToken);
            // Start Kafka server 0
            ExecuteCommandInBackground(_configuration["KafkaConfigs:KafkaClients:KafkaBrokerName1"], @_configuration["KafkaConfigs:KafkaClients:KafkaBrokerBatPath1"], @_configuration["KafkaConfigs:KafkaClients:KafkaBrokerConfigPath1"], cancellationToken);

            // Start Kafka server 1
            ExecuteCommandInBackground(_configuration["KafkaConfigs:KafkaClients:KafkaBrokerName2"], @_configuration["KafkaConfigs:KafkaClients:KafkaBrokerBatPath2"], @_configuration["KafkaConfigs:KafkaClients:KafkaBrokerConfigPath2"], cancellationToken);

            // Start Kafka server 2
            ExecuteCommandInBackground(_configuration["KafkaConfigs:KafkaClients:KafkaBrokerName3"], @_configuration["KafkaConfigs:KafkaClients:KafkaBrokerBatPath3"], @_configuration["KafkaConfigs:KafkaClients:KafkaBrokerConfigPath3"], cancellationToken);

            /*Console.WriteLine("Commands executed in the background. Press any key to exit...");
            Console.ReadLine();*/
        }
        public static void ExecuteCommandInBackground(string processName,string executablePath, string arguments, CancellationToken cancellationToken)
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
            //Set the process name to make it identifiable in the Task Manager
            process.StartInfo.FileName = processName;
            cancellationToken.Register(()=> process.Kill());
        }
    }
}
