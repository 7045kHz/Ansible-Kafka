using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaLogger.DataModels;
public class KafkaEventModel
{
    public string Topic
    {
        get; set;
    }
    public int Partition
    {
        get; set;
    }
    public long Offset
    {
        get; set;
    }
    public DateTime Timestamp
    {
        get; set;
    }
    public string Key
    {
        get; set;
    }
    public string Value
    {
        get; set;
    }
    public KafkaEventModel(string topic, int partition, long offset, DateTime timestamp, string key, string value)
    {
        Topic = topic;
        Partition = partition;
        Offset = offset;
        Timestamp = timestamp;
        Key = key;
        Value = value;
    }
}
