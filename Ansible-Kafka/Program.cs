using KafkaLogger.DataRepository;
 
using KafkaLogger.DataModels;
namespace KafkaLogger;

internal class Program
{
    static void Main(string[] args)
    {
     
        Console.WriteLine("Running Kafka Consumer to DB");
        var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC") ?? null;
        if (String.IsNullOrEmpty(topic))
        {
            Console.WriteLine("DEFINE KAFKA_TOPIC via ENV Variable KAFKA_TOPIC");
            Environment.Exit(1);
        }
        var bootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKER") ?? null;
        if (String.IsNullOrEmpty(bootstrapServers))
        {
            Console.WriteLine("DEFINE KAFKA_BROKER via ENV Variable KAFKA_BROKER");
            Environment.Exit(1);
        }
        var groupId = Environment.GetEnvironmentVariable("KAFKA_GROUP") ?? null;
        if (String.IsNullOrEmpty(groupId))
        {
            Console.WriteLine("DEFINE KAFKA_GROUP via ENV Variable KAFKA_GROUP");
            Environment.Exit(1);
        }
        var startTime = DateTime.UtcNow.AddDays(-10);
        

        KafkaRepository kafkaRepository = new KafkaRepository();
        kafkaRepository.ConsumeMessages(bootstrapServers,groupId, topic);
        
    }
}
