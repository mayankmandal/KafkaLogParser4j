using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaLogProducer
{
    public class Program
    { 
        public static void Main(string[] args)
        {
            // Create a Service Collection for DI
            var serviceCollection = new ServiceCollection();

            // Build a Configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json")
                .Build();
            // Add Configuration to Service Collection 
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddSingleton<KafkaLogProducer>();

            // Test
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var testInstance = serviceProvider.GetService<KafkaLogProducer>();
            testInstance.ProducerMain();
        }
    }
}
