FROM mcr.microsoft.com/dotnet/sdk:6.0

RUN mkdir /api

COPY api /api

WORKDIR /api

EXPOSE 80

RUN dotnet build

ENTRYPOINT ["dotnet", "run", "--project", "api"]