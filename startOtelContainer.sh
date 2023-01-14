#!/bin/bash

clear
docker kill otel
docker container prune --force
echo "Starting OPEN TELEMETRY Container..."

docker run  -d \
            -p 4317:4317 \
            --name otel \
            --hostname OtelContainer_HostMetrics \
            --network sos_net \
            -v $(pwd)/otel_config.yaml:/app/otel_config.yaml \
            otel/opentelemetry-collector-contrib:latest --config /app/otel_config.yaml

docker ps -a