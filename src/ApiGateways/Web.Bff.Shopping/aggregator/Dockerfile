FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src

COPY Dockerfile-scripts Dockerfile-scripts/

COPY src/ApiGateways/ApiGw-Base/*.csproj /src/src/ApiGateways/ApiGw-Base/
COPY src/ApiGateways/Mobile.Bff.Shopping/aggregator/*.csproj /src/src/ApiGateways/Mobile.Bff.Shopping/aggregator/
COPY src/ApiGateways/Web.Bff.Shopping/aggregator/*.csproj /src/src/ApiGateways/Web.Bff.Shopping/aggregator/
COPY src/BuildingBlocks/EventBus/EventBus/*.csproj /src/src/BuildingBlocks/EventBus/EventBus/
COPY src/BuildingBlocks/EventBus/EventBusRabbitMQ/*.csproj /src/src/BuildingBlocks/EventBus/EventBusRabbitMQ/
COPY src/BuildingBlocks/EventBus/EventBusServiceBus/*.csproj /src/src/BuildingBlocks/EventBus/EventBusServiceBus/
COPY src/BuildingBlocks/EventBus/IntegrationEventLogEF/*.csproj /src/src/BuildingBlocks/EventBus/IntegrationEventLogEF/
COPY src/BuildingBlocks/WebHostCustomization/WebHost.Customization/*.csproj /src/src/BuildingBlocks/WebHostCustomization/WebHost.Customization/
COPY src/Services/Basket/Basket.API/*.csproj /src/src/Services/Basket/Basket.API/
COPY src/Services/Catalog/Catalog.API/*.csproj /src/src/Services/Catalog/Catalog.API/
COPY src/Services/Identity/Identity.API/*.csproj /src/src/Services/Identity/Identity.API/
COPY src/Services/Location/Locations.API/*.csproj /src/src/Services/Location/Locations.API/
COPY src/Services/Marketing/Marketing.API/*.csproj /src/src/Services/Marketing/Marketing.API/
COPY src/Services/Ordering/Ordering.API/*.csproj /src/src/Services/Ordering/Ordering.API/
COPY src/Services/Ordering/Ordering.BackgroundTasks/*.csproj /src/src/Services/Ordering/Ordering.BackgroundTasks/
COPY src/Services/Ordering/Ordering.Domain/*.csproj /src/src/Services/Ordering/Ordering.Domain/
COPY src/Services/Ordering/Ordering.Infrastructure/*.csproj /src/src/Services/Ordering/Ordering.Infrastructure/
COPY src/Services/Ordering/Ordering.SignalrHub/*.csproj /src/src/Services/Ordering/Ordering.SignalrHub/
COPY src/Services/Payment/Payment.API/*.csproj /src/src/Services/Payment/Payment.API/
COPY src/Services/Webhooks/Webhooks.API/*.csproj /src/src/Services/Webhooks/Webhooks.API/
COPY src/Web/WebhookClient/*.csproj /src/src/Web/WebhookClient/
COPY src/Web/WebMVC/*.csproj /src/src/Web/WebMVC/
COPY src/Web/WebSPA/*.csproj /src/src/Web/WebSPA/
COPY src/Web/WebStatus/*.csproj /src/src/Web/WebStatus/

RUN Dockerfile-scripts/restore-packages

COPY . .
WORKDIR /src/src/ApiGateways/Web.Bff.Shopping/aggregator
RUN dotnet build --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Web.Shopping.HttpAggregator.dll"]
