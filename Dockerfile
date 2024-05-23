FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /action

COPY ./dist/action_exec ./

ENTRYPOINT ["/action/action_exec"]
