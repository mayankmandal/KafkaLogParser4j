using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaClassLibrary
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

            /*// Main
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Fetch DefaultConnection from appsettings.json
            var defaultConnection = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");

            // Assign DefaultConnection to SqlDBHelper.CONNECTION_STRING
            SqlDBHelper.CONNECTION_STRING = defaultConnection;*/
        }
    }
}
