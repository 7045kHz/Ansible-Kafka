```sql
CREATE VIEW [ETL].[AnsibleEvents_vw] 
  AS
  SELECT ae.Id, ae.Topic,ae.Partition, ae.Offset,ae.Timestamp, ae.[Key],
  [Type] = case WHEN  v.Summary is not null then 'Summary' else 'Task' END,
  v.*, ae.LoadDate 
FROM [ETL].[AnsibleEvents] (NOLOCK) ae
   CROSS APPLY OPENJSON(ae.[Value])
   WITH (
	Event NVARCHAR(200) '$.event',
    [CompletedTimeStamp] NVARCHAR(200)  '$.timestamp',
    Playbook NVARCHAR(200) '$.playbook',
    Task NVARCHAR(500)  '$.task',
	Summary NVARCHAR(max) '$.summary' AS JSON,
	Host NVARCHAR(500)  '$.host'
) v
```