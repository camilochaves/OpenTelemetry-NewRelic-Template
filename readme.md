# OpenTelemetry with New Relic

steps:
* Create a New Relic account or log in to an existing one
* Create a new API key with type Ingest-License
* Open otel_config.yaml and substitute your new API key there under exporters->otlp-> api-key
* start docker: sudo service docker start on WSL
* type: bash startOtelContainer.sh
* dotnet run (to execute the template project)

## Optional

Modify this section of the yml file to point to NP6 Log Directories

```
 filelog:
    include: [ ./logs/*.log ]
    start_at: beginning
    operators:
    - type: regex_parser
      regex: '^(?P<time>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}) (?P<sev>[A-Z]*) (?P<msg>.*)$'
```

Be aware that you need to match the Regex Parser with NP6 Log Structures.
