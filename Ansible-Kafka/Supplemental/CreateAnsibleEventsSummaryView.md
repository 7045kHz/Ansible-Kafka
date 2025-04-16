````sql
CREATE VIEW [ETL].[AnsibleEventsSummary_vw] 
  AS
  SELECT x.[Id],x.[Topic],x.[Partition],x.[Offset],x.[Key],x.[Type],x.[Timestamp],x.[LoadDate] , x.[Host],x.[Playbook],s.*
  FROM (SELECT ae.Id, ae.Topic,ae.Partition, ae.Offset,ae.Timestamp, ae.[Key],
  [Type] = case WHEN  v.Summary is not null then 'Summary' else 'Task' END,
   v.* , ae.LoadDate 
  FROM [PROTO].[ETL].[AnsibleEvents] (NOLOCK) ae
   CROSS APPLY OPENJSON(ae.[Value])
   WITH (
		Event NVARCHAR(200) '$.event',
		[CompletedTimeStamp] NVARCHAR(200)  '$.timestamp',
		Playbook NVARCHAR(200) '$.playbook',
	 Task NVARCHAR(500)  '$.task',
		Summary NVARCHAR(max) '$.summary' AS JSON,
		Host NVARCHAR(500)  '$.host'
	) v 
 ) x 
 CROSS APPLY OPENJSON(x.[Summary])
   WITH (
	Ok int '$.ok',
    Failures int  '$.failures',
    Unreachable int '$.unreachable',
    Skipped int  '$.skipped',
	Rescued int'$.rescued' ,
	Ignored int '$.ignored'
) s

where x.[Type] = 'Summary'
```