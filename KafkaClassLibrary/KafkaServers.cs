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
        //private readonly IConfiguration _configuration;
        public KafkaServers(IConfiguration configuration)
        {
           // _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // Start Kafka Servers
            await KafkaServersMain(cancellationToken);
        }
        public async Task KafkaServersMain(CancellationToken cancellationToken)
        {
            // Start Zookeeper server
            ExecuteCommandInBackground("ZooKeeperServer",@"C:\kafka_2.12-3.5.1\bin\windows\zookeeper-server-start.bat", @"C:\kafka_2.12-3.5.1\config\zookeeper.properties", cancellationToken);
            await Task.Delay(TimeSpan.FromSeconds(45), cancellationToken);
            // Start Kafka server 0
            ExecuteCommandInBackground("KafkaBroker1",@"C:\kafka_2.12-3.5.1\bin\windows\kafka-server-start.bat", @"C:\kafka_2.12-3.5.1\config\server-0.properties", cancellationToken);

            // Start Kafka server 1
            ExecuteCommandInBackground("KafkaBroker2",@"C:\kafka_2.12-3.5.1\bin\windows\kafka-server-start.bat", @"C:\kafka_2.12-3.5.1\config\server-1.properties", cancellationToken);

            // Start Kafka server 2
            ExecuteCommandInBackground("KafkaBroker3",@"C:\kafka_2.12-3.5.1\bin\windows\kafka-server-start.bat", @"C:\kafka_2.12-3.5.1\config\server-2.properties", cancellationToken);

            Console.WriteLine("Commands executed in the background. Press any key to exit...");
            Console.ReadLine();
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
                WorkingDirectory = @"C:\kafka_2.12-3.5.1\bin\windows\",
            };

            Process process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
            process.Start();
            //Set the process name to make it identifiable in the Task Manager
            process.StartInfo.FileName = processName;
            cancellationToken.Register(()=> process.Kill());
        }
    }
}
