# .NET diagnostics lab

Aspnet app with diagnostics samples using:
- [`dotnet-counters`](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-counters)
- [`dotnet-stack`](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-stack)
- [`hey`](https://github.com/rakyll/hey)
- [`dotnet-monitor`](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-monitor)
- [`prometheus`](https://prometheus.io/)
- [`grafana`](https://grafana.com/docs/grafana/latest/)

## Step by step - dotnet-monitor + prometheus + grafana

Running the app will start a container with `dotnet-monitor`. This container is setup to get diagnostic commands from the app container through a [diagnostic port](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/diagnostic-port). Dotnet monitor exposes endpoints with usefull information:

- http://localhost:52323/info
- http://localhost:52323/processes
- http://localhost:52325/metrics

A full list can be seem here: https://github.com/dotnet/dotnet-monitor/blob/main/documentation/api/README.md.

The metrics endpoint captures metrics in the Prometheus exposition format.

To start grafana use the command bellow:

```
docker compose up grafana
```

The compose provisions grafana with prometheus datasource and the community provided [dotnet-monitor dashboard](https://grafana.com/grafana/dashboards/19297-dotnet-monitor-dashboard/). Once is started up go to http://localhost:3000/dashboards (user: `admin`, password: `admin`)


## Load tests

Run the load test:

```sh
docker compose -f docker-compose.yml -f compose.tests.yml run --rm send-load-sync
docker compose -f docker-compose.yml -f compose.tests.yml run --rm send-load-sync
docker compose -f docker-compose.yml -f compose.tests.yml run --rm send-load-enumeration
docker compose -f docker-compose.yml -f compose.tests.yml run --rm send-load-multiple-enumeration
```

## Egress configuration

dotnet-monitor allows setting the egress to export artifacts like dumps. By default it's used the filesystem.

Configuration to use AWS S3

```yml
DOTNETMONITOR_Egress__S3Storage__monitorS3Blob__bucketName: bucketname
DOTNETMONITOR_Egress__S3Storage__monitorS3Blob__accessKeyId: accesskeyid
DOTNETMONITOR_Egress__S3Storage__monitorS3Blob__secretAccessKey: secretaccesskey
DOTNETMONITOR_Egress__S3Storage__monitorS3Blob__regionName: us-east-1
```

And the parameter `egressProvider` should be `monitorS3Blob`.

```sh
curl -X 'GET' \
  'http://localhost:52323/dump?egressProvider=monitorS3Blob' \
  -H 'accept: application/octet-stream'
```

[Docs](https://github.com/dotnet/dotnet-monitor/blob/407ddc545b08fce1eb245c8c16ca46316fb8af78/documentation/configuration/egress-configuration.md)