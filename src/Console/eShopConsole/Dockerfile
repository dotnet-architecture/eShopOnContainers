FROM microsoft/dotnet:1.0.1-core
ENTRYPOINT ["dotnet", "eShopConsole.dll"]
ARG source=.
WORKDIR /app
COPY $source .
