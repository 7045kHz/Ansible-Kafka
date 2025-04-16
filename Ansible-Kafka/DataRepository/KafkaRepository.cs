using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Confluent.Kafka;
using KafkaLogger.DataModels;

namespace KafkaLogger.DataRepository;
public class KafkaRepository
{
    public void ConsumeMessagesStartSince(string bootstrapServers, string groupId, string topic, DateTime startTime)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            SessionTimeoutMs = 6000,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnablePartitionEof = true,
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false,
        };
        using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
        {
            consumer.Subscribe(topic);

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            // Get the partitions for the topic

            var partitions = consumer.Assignment;
            if (partitions.Count == 0)
            {
                partitions = consumer.Assignment;
                consumer.Assign(partitions);
            }
            // Create a list of TopicPartitionTimestamp for the desired start time

            var timestampOffsets = partitions.Select(partition => new TopicPartitionTimestamp(partition, new Timestamp(startTime))).ToList();
            // Fetch offsets for the specified timestamps
            var offsets = consumer.OffsetsForTimes(timestampOffsets, TimeSpan.FromSeconds(10));

            // Assign the consumer to the calculated offsets
            foreach (var offset in offsets)
            {
                if (offset.Offset != Offset.Unset)
                {
                    consumer.Seek(offset);
                }
            }
            Console.WriteLine($"Consuming messages from {startTime}...");

            try
            {
                while (true)
                {
                    var result = consumer.Consume(cts.Token);
                    if (result.IsPartitionEOF)
                    {
                        Console.WriteLine($"End of partition reached: {result.TopicPartitionOffset}");
                        continue;
                    }
                    if (!string.IsNullOrEmpty(result.Message.Value))
                    {
                        Console.WriteLine($"Message: {result.Message.Value}, Partition: {result.Partition}, Offset: {result.Offset}");
                    }
                    else
                    {
                        Console.WriteLine($"Message: '', Partition: {result.Partition}, Offset: {result.Offset}");
                    }
                }

            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Error occured: {e.Error.Reason}");
            }
            finally
            {

                consumer.Close();

            }
        }
    }
 

    public void ConsumeMessages(string bootstrapServers, string groupId, string topic)
    {
        var DbRepository = new DbRepository();
        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false,
        };
        using (var c = new ConsumerBuilder<Ignore, string>(config).Build())
        {
            c.Subscribe(topic);

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            try
            {
                while (true)
                {
                    try
                    {
                        var cr = c.Consume(cts.Token);
                        // Commit the offset of the consumed message
                        var key = string.Empty;
                        if (string.IsNullOrEmpty(cr.Message.Key?.ToString())  )
                        {
                            key = "Ansible";
                        }
                        else
                        {
                            key = cr.Message.Key?.ToString();
                        }
                        var kafkaEvent = new KafkaEventModel(cr.Topic, cr.Partition, cr.Offset.Value, cr.Message.Timestamp.UtcDateTime, key, cr.Message.Value.ToString());
                        Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                        var ret = DbRepository.InsertKafkaMessageData(kafkaEvent);
                        if(ret >= 0)
                        {
                            c.Commit(cr);
                            Console.WriteLine($"Inserted message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");
                        }
                        

                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occured: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Close and Release all the resources held by this consumer  
                c.Close();
            }
        }
    }
}
