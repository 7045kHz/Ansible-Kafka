```sql

/* Example of a SQL script to create a table for storing Ansible events. */
CREATE TABLE [ETL].[AnsibleEvents](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Topic] [nvarchar](250) NULL,
	[Partition] int NULL,
	[Offset] int  NULL,
	[Timestamp] DateTime NULL,
	[Key] [nvarchar](250) NULL,
	[Value] [nvarchar](max) NULL,
	[LoadDate] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP
);

```