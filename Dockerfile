FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /action

COPY ./dist/S3DeploymentAction ./init

ENTRYPOINT ["/action/init"]
