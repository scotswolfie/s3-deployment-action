FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app

COPY ./dist/S3DeploymentAction ./init

ENTRYPOINT ["./init"]
