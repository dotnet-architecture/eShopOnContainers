FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY eShopOnContainers-ServicesAndWebApps.sln ./
COPY src/Services/Catalog/Catalog.API/Catalog.API.csproj src/Services/Catalog/Catalog.API/
COPY src/BuildingBlocks/HealthChecks/src/Microsoft.AspNetCore.HealthChecks/Microsoft.AspNetCore.HealthChecks.csproj src/BuildingBlocks/HealthChecks/src/Microsoft.AspNetCore.HealthChecks/
COPY src/BuildingBlocks/HealthChecks/src/Microsoft.Extensions.HealthChecks/Microsoft.Extensions.HealthChecks.csproj src/BuildingBlocks/HealthChecks/src/Microsoft.Extensions.HealthChecks/
COPY src/BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj src/BuildingBlocks/EventBus/IntegrationEventLogEF/
COPY src/BuildingBlocks/EventBus/EventBus/EventBus.csproj src/BuildingBlocks/EventBus/EventBus/
COPY src/BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj src/BuildingBlocks/EventBus/EventBusRabbitMQ/
COPY src/BuildingBlocks/EventBus/EventBusServiceBus/EventBusServiceBus.csproj src/BuildingBlocks/EventBus/EventBusServiceBus/
COPY src/BuildingBlocks/WebHostCustomization/WebHost.Customization/WebHost.Customization.csproj src/BuildingBlocks/WebHostCustomization/WebHost.Customization/
COPY src/BuildingBlocks/HealthChecks/src/Microsoft.Extensions.HealthChecks.SqlServer/Microsoft.Extensions.HealthChecks.SqlServer.csproj src/BuildingBlocks/HealthChecks/src/Microsoft.Extensions.HealthChecks.SqlServer/
COPY src/BuildingBlocks/HealthChecks/src/Microsoft.Extensions.HealthChecks.AzureStorage/Microsoft.Extensions.HealthChecks.AzureStorage.csproj src/BuildingBlocks/HealthChecks/src/Microsoft.Extensions.HealthChecks.AzureStorage/
RUN dotnet restore
COPY . .
WORKDIR /src/src/Services/Catalog/Catalog.API
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
