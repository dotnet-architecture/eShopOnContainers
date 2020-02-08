FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src

# Keep the project list and command dotnet restore identical in all Dockfiles to maximize image cache utilization
COPY eShopOnContainers-ServicesAndWebApps.sln .
COPY docker-compose.dcproj /src/
COPY src/ApiGateways/ApiGw-Base/OcelotApiGw.csproj src/ApiGateways/ApiGw-Base/
COPY src/ApiGateways/Mobile.Bff.Shopping/aggregator/Mobile.Shopping.HttpAggregator.csproj src/ApiGateways/Mobile.Bff.Shopping/aggregator/
COPY src/ApiGateways/Web.Bff.Shopping/aggregator/Web.Shopping.HttpAggregator.csproj src/ApiGateways/Web.Bff.Shopping/aggregator/
COPY src/BuildingBlocks/Devspaces.Support/Devspaces.Support.csproj src/BuildingBlocks/Devspaces.Support/
COPY src/BuildingBlocks/EventBus/EventBus/EventBus.csproj src/BuildingBlocks/EventBus/EventBus/
COPY src/BuildingBlocks/EventBus/EventBus.Tests/EventBus.Tests.csproj src/BuildingBlocks/EventBus/EventBus.Tests/
COPY src/BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj src/BuildingBlocks/EventBus/EventBusRabbitMQ/
COPY src/BuildingBlocks/EventBus/EventBusServiceBus/EventBusServiceBus.csproj src/BuildingBlocks/EventBus/EventBusServiceBus/
COPY src/BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj src/BuildingBlocks/EventBus/IntegrationEventLogEF/
COPY src/BuildingBlocks/WebHostCustomization/WebHost.Customization/WebHost.Customization.csproj src/BuildingBlocks/WebHostCustomization/WebHost.Customization/
COPY src/Services/Basket/Basket.API/Basket.API.csproj src/Services/Basket/Basket.API/
COPY src/Services/Basket/Basket.FunctionalTests/Basket.FunctionalTests.csproj src/Services/Basket/Basket.FunctionalTests/
COPY src/Services/Basket/Basket.UnitTests/Basket.UnitTests.csproj src/Services/Basket/Basket.UnitTests/
COPY src/Services/Catalog/Catalog.API/Catalog.API.csproj src/Services/Catalog/Catalog.API/
COPY src/Services/Catalog/Catalog.FunctionalTests/Catalog.FunctionalTests.csproj src/Services/Catalog/Catalog.FunctionalTests/
COPY src/Services/Catalog/Catalog.UnitTests/Catalog.UnitTests.csproj src/Services/Catalog/Catalog.UnitTests/
COPY src/Services/Identity/Identity.API/Identity.API.csproj src/Services/Identity/Identity.API/
COPY src/Services/Location/Locations.API/Locations.API.csproj src/Services/Location/Locations.API/
COPY src/Services/Location/Locations.FunctionalTests/Locations.FunctionalTests.csproj src/Services/Location/Locations.FunctionalTests/
COPY src/Services/Marketing/Marketing.API/Marketing.API.csproj src/Services/Marketing/Marketing.API/
COPY src/Services/Marketing/Marketing.FunctionalTests/Marketing.FunctionalTests.csproj src/Services/Marketing/Marketing.FunctionalTests/
COPY src/Services/Ordering/Ordering.API/Ordering.API.csproj src/Services/Ordering/Ordering.API/
COPY src/Services/Ordering/Ordering.BackgroundTasks/Ordering.BackgroundTasks.csproj src/Services/Ordering/Ordering.BackgroundTasks/
COPY src/Services/Ordering/Ordering.Domain/Ordering.Domain.csproj src/Services/Ordering/Ordering.Domain/
COPY src/Services/Ordering/Ordering.FunctionalTests/Ordering.FunctionalTests.csproj src/Services/Ordering/Ordering.FunctionalTests/
COPY src/Services/Ordering/Ordering.Infrastructure/Ordering.Infrastructure.csproj src/Services/Ordering/Ordering.Infrastructure/
COPY src/Services/Ordering/Ordering.SignalrHub/Ordering.SignalrHub.csproj src/Services/Ordering/Ordering.SignalrHub/
COPY src/Services/Ordering/Ordering.UnitTests/Ordering.UnitTests.csproj src/Services/Ordering/Ordering.UnitTests/
COPY src/Services/Payment/Payment.API/Payment.API.csproj src/Services/Payment/Payment.API/
COPY src/Services/Webhooks/Webhooks.API/Webhooks.API.csproj src/Services/Webhooks/Webhooks.API/
COPY src/Web/WebhookClient/WebhookClient.csproj src/Web/WebhookClient/
COPY src/Web/WebMVC/WebMVC.csproj src/Web/WebMVC/
COPY src/Web/WebSPA/WebSPA.csproj src/Web/WebSPA/
COPY src/Web/WebStatus/WebStatus.csproj src/Web/WebStatus/
COPY test/ServicesTests/Application.FunctionalTests/Application.FunctionalTests.csproj test/ServicesTests/Application.FunctionalTests/
COPY test/ServicesTests/LoadTest/LoadTest.csproj test/ServicesTests/LoadTest/

RUN dotnet restore eShopOnContainers-ServicesAndWebApps.sln

COPY . .
WORKDIR /src/src/Services/TenantCustomisations/TenantAShippingInformation 
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TenantAShippingInformation.dll"]
