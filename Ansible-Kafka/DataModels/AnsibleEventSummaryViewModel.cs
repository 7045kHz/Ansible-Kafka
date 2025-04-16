using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaLogger.DataModels;
public class AnsibleEventSummaryViewModel
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
    public string Key
    {
        get; set;
    }

    public string Type
    {
        get; set;
    }
    public DateTime Timestamp
    {
        get; set;
    }
   
    public DateTime LoadDate
    {
        get; set;
    }

    public string Host
    {
        get; set;
    }
    public string Playbook
    {
        get; set;
    }
    public int Ok
    {
        get; set;
    }
    public int Failures
    {
        get; set;
    }
    public int Unreachable
    {
        get; set;
    }
    public int Skipped
    {
        get; set;
    }
    public int Rescued
    {
        get; set;
    }
    public int Ignored
    {
        get; set;
    }
}
