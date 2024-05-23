FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app

COPY ./artifacts/publish/S3DeploymentAction/release_linux-x64/S3DeploymentAction ./init

ENTRYPOINT ["./init"]
