# Creating an Ansible AAP Logging to Kafka, with a MicroService that writes to SQL Server
## This document provides instructions for setting up Ansible AAP logging to Kafka and code for a Kafka to SQL Server MicroService.

## Prerequisites
- Ansible Execution Environment created with Kafka tools installed. [Execution Environment](./AnsibleExecutionEnvironment.md)
- Ansible Callback plugin created and configured to send logs to Kafka. [Ansible Callback](./Ansible_Callback_Example_py.md)
- Docker installed on your machine.
- Table, account, and database created in your SQL Server instance. [SQL Table](./CreateAnsibleEventsTable.md)
- Environment variables set in a `.env` file or directly in the shell.
- Source environment variables from the `.env` file before running the Docker image.

## Known enhancements needed
- Add better error handling for Kafka connection issues.
- Add better error handling for SQL connection issues.
- Clean Architecture: Separate the code into different layers (e.g., repository, interfaces, service, controller).
- Improve options for Kafka connection settings (e.g., SSL, authentication).
- Better example view for SQL side data. [SQL AnsibleEvents_vw](./CreateAnsibleEventsView.md)]
- Better example view for SQL side data. [SQL AnsibleEventsSummary_vw](./CreateAnsibleEventsSummaryView.md)]

## Example SQL Table Data
 [ETL.AnsibleEvents.csv](./etl_ansible_events.csv)

 |Topic       |Partition|Offset|Timestamp              |Key    |Value                                                                                                                                                                                                                                                  |
|------------|---------|------|-----------------------|-------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
|ansible_logs|0        |534   |2025-04-16 12:46:54.303|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:46:54.303211", "playbook": "unix_patch_dev.yml", "task": "Gathering Facts", "host": "developer.example.com"}                                                                                       |
|ansible_logs|0        |535   |2025-04-16 12:46:54.310|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:46:54.309465", "playbook": "unix_patch_dev.yml", "task": "Gathering Facts", "host": "prometheus.example.com"}                                                                                      |
|ansible_logs|0        |536   |2025-04-16 12:46:59.327|Ansible|{"event": "task_ok", "timestamp": "2025-04-16T12:46:59.326594", "playbook": "unix_patch_dev.yml", "task": "Gathering Facts", "host": "api.example.com"}                                                                                                |
|ansible_logs|0        |537   |2025-04-16 12:46:59.370|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:46:59.369582", "playbook": "unix_patch_dev.yml", "task": "Make sure yum-utils are installed", "host": "prometheus.example.com"}                                                                    |
|ansible_logs|0        |538   |2025-04-16 12:47:22.540|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:47:22.539963", "playbook": "unix_patch_dev.yml", "task": "update the system", "host": "developer.example.com"}                                                                                     |
|ansible_logs|0        |539   |2025-04-16 12:47:34.197|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:47:34.198340", "playbook": "unix_patch_dev.yml", "task": "check to see if we need a reboot", "host": "developer.example.com"}                                                                      |
|ansible_logs|0        |540   |2025-04-16 12:47:34.207|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:47:34.205789", "playbook": "unix_patch_dev.yml", "task": "check to see if we need a reboot", "host": "prometheus.example.com"}                                                                     |
|ansible_logs|0        |541   |2025-04-16 12:47:36.817|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:47:36.818086", "playbook": "unix_patch_dev.yml", "task": "wait for the system to reboot", "host": "prometheus.example.com"}                                                                        |
|ansible_logs|0        |542   |2025-04-16 12:47:36.820|Ansible|{"event": "task_skipped", "timestamp": "2025-04-16T12:47:36.821046", "playbook": "unix_patch_dev.yml", "task": "wait for the system to reboot", "host": "api.example.com"}                                                                             |
|ansible_logs|0        |543   |2025-04-16 12:47:36.820|Ansible|{"event": "task_skipped", "timestamp": "2025-04-16T12:47:36.821746", "playbook": "unix_patch_dev.yml", "task": "wait for the system to reboot", "host": "developer.example.com"}                                                                       |
|ansible_logs|0        |544   |2025-04-16 12:47:36.827|Ansible|{"event": "task_skipped", "timestamp": "2025-04-16T12:47:36.827725", "playbook": "unix_patch_dev.yml", "task": "wait for the system to reboot", "host": "prometheus.example.com"}                                                                      |
|ansible_logs|1        |554   |2025-04-16 12:46:55.417|Ansible|{"event": "task_ok", "timestamp": "2025-04-16T12:46:55.416654", "playbook": "unix_patch_dev.yml", "task": "Gathering Facts", "host": "developer.example.com"}                                                                                          |
|ansible_logs|1        |555   |2025-04-16 12:46:55.513|Ansible|{"event": "task_ok", "timestamp": "2025-04-16T12:46:55.513698", "playbook": "unix_patch_dev.yml", "task": "Gathering Facts", "host": "prometheus.example.com"}                                                                                         |
|ansible_logs|1        |556   |2025-04-16 12:47:24.717|Ansible|{"event": "task_ok", "timestamp": "2025-04-16T12:47:24.718972", "playbook": "unix_patch_dev.yml", "task": "update the system", "host": "prometheus.example.com"}                                                                                       |
|ansible_logs|1        |557   |2025-04-16 12:47:29.127|Ansible|{"event": "task_ok", "timestamp": "2025-04-16T12:47:29.128381", "playbook": "unix_patch_dev.yml", "task": "update the system", "host": "developer.example.com"}                                                                                        |
|ansible_logs|1        |558   |2025-04-16 12:47:35.257|Ansible|{"event": "task_ok", "timestamp": "2025-04-16T12:47:35.257184", "playbook": "unix_patch_dev.yml", "task": "check to see if we need a reboot", "host": "prometheus.example.com"}                                                                        |
|ansible_logs|1        |559   |2025-04-16 12:47:36.307|Ansible|{"event": "task_ok", "timestamp": "2025-04-16T12:47:36.306119", "playbook": "unix_patch_dev.yml", "task": "check to see if we need a reboot", "host": "api.example.com"}                                                                               |
|ansible_logs|1        |560   |2025-04-16 12:47:36.710|Ansible|{"event": "task_ok", "timestamp": "2025-04-16T12:47:36.711616", "playbook": "unix_patch_dev.yml", "task": "check to see if we need a reboot", "host": "developer.example.com"}                                                                         |
|ansible_logs|1        |561   |2025-04-16 12:47:36.717|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:47:36.718310", "playbook": "unix_patch_dev.yml", "task": "display result", "host": "api.example.com"}                                                                                              |
|ansible_logs|1        |562   |2025-04-16 12:47:36.727|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:47:36.726264", "playbook": "unix_patch_dev.yml", "task": "display result", "host": "developer.example.com"}                                                                                        |
|ansible_logs|1        |563   |2025-04-16 12:47:36.737|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:47:36.735565", "playbook": "unix_patch_dev.yml", "task": "display result", "host": "prometheus.example.com"}                                                                                       |
|ansible_logs|1        |564   |2025-04-16 12:47:36.740|Ansible|{"event": "task_ok", "timestamp": "2025-04-16T12:47:36.739167", "playbook": "unix_patch_dev.yml", "task": "display result", "host": "api.example.com"}                                                                                                 |
|ansible_logs|1        |565   |2025-04-16 12:47:36.740|Ansible|{"event": "task_ok", "timestamp": "2025-04-16T12:47:36.740009", "playbook": "unix_patch_dev.yml", "task": "display result", "host": "developer.example.com"}                                                                                           |
|ansible_logs|1        |566   |2025-04-16 12:47:36.750|Ansible|{"event": "task_ok", "timestamp": "2025-04-16T12:47:36.749348", "playbook": "unix_patch_dev.yml", "task": "display result", "host": "prometheus.example.com"}                                                                                          |
|ansible_logs|1        |567   |2025-04-16 12:47:36.757|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:47:36.755524", "playbook": "unix_patch_dev.yml", "task": "restart system to reboot to newest kernel", "host": "api.example.com"}                                                                   |
|ansible_logs|1        |568   |2025-04-16 12:47:36.763|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:47:36.763064", "playbook": "unix_patch_dev.yml", "task": "restart system to reboot to newest kernel", "host": "developer.example.com"}                                                             |
|ansible_logs|1        |569   |2025-04-16 12:47:36.770|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:47:36.771169", "playbook": "unix_patch_dev.yml", "task": "restart system to reboot to newest kernel", "host": "prometheus.example.com"}                                                            |
|ansible_logs|1        |570   |2025-04-16 12:47:36.800|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:47:36.801383", "playbook": "unix_patch_dev.yml", "task": "wait for the system to reboot", "host": "api.example.com"}                                                                               |
|ansible_logs|1        |571   |2025-04-16 12:47:36.807|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:47:36.808835", "playbook": "unix_patch_dev.yml", "task": "wait for the system to reboot", "host": "developer.example.com"}                                                                         |
|ansible_logs|2        |474   |2025-04-16 12:46:54.287|Ansible|{"event": "playbook_start", "timestamp": "2025-04-16T12:46:54.288601", "playbook": "unix_patch_dev.yml"}                                                                                                                                               |
|ansible_logs|2        |475   |2025-04-16 12:46:54.297|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:46:54.297496", "playbook": "unix_patch_dev.yml", "task": "Gathering Facts", "host": "api.example.com"}                                                                                             |
|ansible_logs|2        |476   |2025-04-16 12:46:59.353|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:46:59.353635", "playbook": "unix_patch_dev.yml", "task": "Make sure yum-utils are installed", "host": "api.example.com"}                                                                           |
|ansible_logs|2        |477   |2025-04-16 12:46:59.363|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:46:59.361971", "playbook": "unix_patch_dev.yml", "task": "Make sure yum-utils are installed", "host": "developer.example.com"}                                                                     |
|ansible_logs|2        |478   |2025-04-16 12:47:00.780|Ansible|{"event": "task_ok", "timestamp": "2025-04-16T12:47:00.781626", "playbook": "unix_patch_dev.yml", "task": "Make sure yum-utils are installed", "host": "prometheus.example.com"}                                                                       |
|ansible_logs|2        |479   |2025-04-16 12:47:04.073|Ansible|{"event": "task_ok", "timestamp": "2025-04-16T12:47:04.074873", "playbook": "unix_patch_dev.yml", "task": "Make sure yum-utils are installed", "host": "developer.example.com"}                                                                        |
|ansible_logs|2        |480   |2025-04-16 12:47:22.527|Ansible|{"event": "task_ok", "timestamp": "2025-04-16T12:47:22.526006", "playbook": "unix_patch_dev.yml", "task": "Make sure yum-utils are installed", "host": "api.example.com"}                                                                              |
|ansible_logs|2        |481   |2025-04-16 12:47:22.533|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:47:22.532317", "playbook": "unix_patch_dev.yml", "task": "update the system", "host": "api.example.com"}                                                                                           |
|ansible_logs|2        |482   |2025-04-16 12:47:22.550|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:47:22.550826", "playbook": "unix_patch_dev.yml", "task": "update the system", "host": "prometheus.example.com"}                                                                                    |
|ansible_logs|2        |483   |2025-04-16 12:47:34.183|Ansible|{"event": "task_ok", "timestamp": "2025-04-16T12:47:34.184556", "playbook": "unix_patch_dev.yml", "task": "update the system", "host": "api.example.com"}                                                                                              |
|ansible_logs|2        |484   |2025-04-16 12:47:34.190|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:47:34.190724", "playbook": "unix_patch_dev.yml", "task": "check to see if we need a reboot", "host": "api.example.com"}                                                                            |
|ansible_logs|2        |485   |2025-04-16 12:47:36.773|Ansible|{"event": "task_skipped", "timestamp": "2025-04-16T12:47:36.774212", "playbook": "unix_patch_dev.yml", "task": "restart system to reboot to newest kernel", "host": "api.example.com"}                                                                 |
|ansible_logs|2        |486   |2025-04-16 12:47:36.773|Ansible|{"event": "task_skipped", "timestamp": "2025-04-16T12:47:36.774927", "playbook": "unix_patch_dev.yml", "task": "restart system to reboot to newest kernel", "host": "developer.example.com"}                                                           |
|ansible_logs|2        |487   |2025-04-16 12:47:36.780|Ansible|{"event": "task_skipped", "timestamp": "2025-04-16T12:47:36.780050", "playbook": "unix_patch_dev.yml", "task": "restart system to reboot to newest kernel", "host": "prometheus.example.com"}                                                          |
|ansible_logs|2        |488   |2025-04-16 12:47:36.787|Ansible|{"event": "task_start", "timestamp": "2025-04-16T12:47:36.786165", "playbook": "unix_patch_dev.yml", "task": "wait for 10 seconds", "host": "api.example.com"}                                                                                         |
|ansible_logs|2        |489   |2025-04-16 12:47:36.797|Ansible|{"event": "task_skipped", "timestamp": "2025-04-16T12:47:36.795254", "playbook": "unix_patch_dev.yml", "task": "wait for 10 seconds", "host": "api.example.com"}                                                                                       |
|ansible_logs|2        |490   |2025-04-16 12:47:36.870|Ansible|{"event": "playbook_end", "timestamp": "2025-04-16T12:47:36.870613", "playbook": "unix_patch_dev.yml", "summary": {"ok": 5, "failures": 0, "unreachable": 0, "changed": 2, "skipped": 2, "rescued": 0, "ignored": 0}, "host": "developer.example.com"} |
|ansible_logs|2        |491   |2025-04-16 12:47:36.870|Ansible|{"event": "playbook_end", "timestamp": "2025-04-16T12:47:36.871058", "playbook": "unix_patch_dev.yml", "summary": {"ok": 5, "failures": 0, "unreachable": 0, "changed": 1, "skipped": 2, "rescued": 0, "ignored": 0}, "host": "prometheus.example.com"}|
|ansible_logs|2        |492   |2025-04-16 12:47:36.870|Ansible|{"event": "playbook_end", "timestamp": "2025-04-16T12:47:36.871254", "playbook": "unix_patch_dev.yml", "summary": {"ok": 5, "failures": 0, "unreachable": 0, "changed": 1, "skipped": 3, "rescued": 0, "ignored": 0}, "host": "api.example.com"}       |

## Example Logged JSON
JSON inserted into [Value] field, with respective Topic, Partition, Offset, Timestamp, LoadDate and TBD (key) fields populated for each insert.

```json
{"event": "playbook_start", "timestamp": "2025-04-14T20:41:16.544553", "playbook": "unix_patch_dev.yml"}
{"event": "task_start", "timestamp": "2025-04-14T20:41:16.552875", "playbook": "unix_patch_dev.yml", "task": "Gathering Facts", "host": "server1.example.com"}
{"event": "task_ok", "timestamp": "2025-04-14T20:41:17.648940", "playbook": "unix_patch_dev.yml", "task": "Gathering Facts", "host": "server1.example.com"}
{"event": "task_start", "timestamp": "2025-04-14T20:41:16.552875", "playbook": "unix_patch_dev.yml", "task": "Gathering Facts", "host": "server2.example.com"}
{"event": "task_ok", "timestamp": "2025-04-14T20:41:17.648940", "playbook": "unix_patch_dev.yml", "task": "Gathering Facts", "host": "server2.example.com"}
{"event": "task_start", "timestamp": "2025-04-14T20:46:51.481674", "playbook": "unix_patch_dev.yml", "task": "restart system to reboot to newest kernel", "host": "server1.example.com"}
{"event": "task_skipped", "timestamp": "2025-04-14T20:41:54.257424", "playbook": "unix_patch_dev.yml", "task": "restart system to reboot to newest kernel", "host": "server2.example.com"}
{"event": "playbook_end", "timestamp": "2025-04-14T20:46:51.585759", "playbook": "unix_patch_dev.yml", "summary": {"ok": 5, "failures": 0, "unreachable": 0, "changed": 1, "skipped": 3, "rescued": 0, "ignored": 0}, "host": "server1.example.com"}

```
## Example table
```sql

# If you want to change this name, you will need to update the source code DbRepository.cs

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
# Example .env file

```sh
# Example .env file
cat .env

export DB_CONNECTION_STRING="Server=mydbserver;Database=mydb;User Id=myuser;Password=mypassword;Trusted_Connection=false;TrustServerCertificate=True;Encrypt=False"
export KAFKA_GROUP="my-consumer-group2"
export KAFKA_BROKER="mykafkaserver.example.com:9092"
export KAFKA_TOPIC="ansible_logs"
export KAFKA_RECOVERY_GROUP="my-recovery-group"

```
## Run the Docker Image using Podman with TLS verification disabled and docker aliases installed
```sh

. my.env

 docker run -d   --tls-verify=false --restart always  artifactory.example.com:5000/kafkalogger:latest

```


