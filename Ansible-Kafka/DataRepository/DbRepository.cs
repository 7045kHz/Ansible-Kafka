using System;

using Dapper;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using KafkaLogger.DataModels;
using System.Text.Json;

namespace KafkaLogger.DataRepository;
public class DbRepository
{
    internal string? _connectionString = string.Empty;
    public DbRepository()
    {
        // Initialize the connection string here or pass it as a parameter
        if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")))
        {
            _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        }
        else
        {
            Console.WriteLine("DEFINE DB Connection string via ENV Variable DB_CONNECTION_STRING");
            Environment.Exit(1);
        }


    }

    public IEnumerable<AnsibleEventSummaryViewModel> GetAnsibleEventsSummaryViewByTopic(string topic)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var selectQuery = "SELECT * FROM [ETL].[AnsibleEventsSummary_vw] (NOLOCK) WHERE [Topic] = @Topic";
            var result = connection.Query<AnsibleEventSummaryViewModel>(selectQuery, new { Topic = topic });
            return result;
        }
    }
    public IEnumerable<AnsibleEventSummaryViewModel> GetAnsibleEventsSummaryViewByHost(string host)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var selectQuery = "SELECT * FROM [ETL].[AnsibleEventsSummary_vw] (NOLOCK) WHERE [Host] = @Host";
            var result = connection.Query<AnsibleEventSummaryViewModel>(selectQuery, new { Host = host });
            return result;
        }
    }
    public IEnumerable<AnsibleEventSummaryViewModel> GetAnsibleEventSummaryViewAll()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var selectQuery = "SELECT * FROM [ETL].[AnsibleEventsSummary_vw] (NOLOCK) ";
            var result = connection.Query<AnsibleEventSummaryViewModel>(selectQuery);
            return result;
        }
    }
    public IEnumerable<AnsibleEventsViewModel> GetAnsibleEventsViewAll()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var selectQuery = "SELECT * FROM [ETL].[AnsibleEvents_vw] (NOLOCK) ";
            var result = connection.Query<AnsibleEventsViewModel>(selectQuery);
            return result;
        }
    }
    public IEnumerable<AnsibleEventsViewModel> GetAnsibleEventsViewByTopic(string topic)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var selectQuery = "SELECT * FROM [ETL].[AnsibleEvents_vw]  (NOLOCK) WHERE [Topic] = @Topic";
            var result = connection.Query<AnsibleEventsViewModel>(selectQuery, new { Topic = topic });
            return result;
        }
    }
    public IEnumerable<AnsibleEventsViewModel> GetAnsibleEventsViewByHost(string host)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var selectQuery = "SELECT * FROM [ETL].[AnsibleEvents_vw]  (NOLOCK) WHERE [Host] = @Host";
            var result = connection.Query<AnsibleEventsViewModel>(selectQuery, new { Host = host });
            return result;
        }
    }
    public IEnumerable<KafkaEventModel> GetRawKafkaMessageDataByTopic(string topic)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var selectQuery = "SELECT * FROM [ETL].[AnsibleEvents] WHERE [Topic] = @Topic";
            var result = connection.Query<KafkaEventModel>(selectQuery, new { Topic = topic });
            return result;
        }
    }
    public int InsertKafkaMessageData(KafkaEventModel ansibleEvent)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var insertQuery = "INSERT INTO [ETL].[AnsibleEvents] ([Topic] , [Partition] ,[Offset] ,[Timestamp] ,[Key],[Value] ) VALUES (@Topic , @Partition ,@Offset ,@Timestamp ,@Key,@Value)";
            var result = connection.Execute(insertQuery, ansibleEvent);
            return result;
        }

    }
}

