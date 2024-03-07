using Microsoft.Extensions.Configuration;

namespace KafkaClassLibrary
{
    public static class SharedVariables
    {
        private static IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
        public static string InputTopic = configuration.GetSection("FirstTopicName").Value;
        public static string OutputTopic = configuration.GetSection("SecondTopicName").Value;
        public static bool IsInputTopicCreated = false;
        public static bool IsOutputTopicCreated = false;
    }
}
