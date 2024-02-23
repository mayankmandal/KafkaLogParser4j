using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaLogEnricher
{
    public class Program
    {
        public static void Main()
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
            serviceCollection.AddSingleton<KafkaLogEnricher>();

            // Test
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var instance = serviceProvider.GetService<KafkaLogEnricher>();
            instance.EnricherMain();
        }
    }
}
