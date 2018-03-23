FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY eShopOnContainers-ServicesAndWebApps.sln ./
COPY test/Services/UnitTest/UnitTest.csproj test/Services/UnitTest/
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
COPY src/Web/WebMVC/WebMVC.csproj src/Web/WebMVC/
COPY src/BuildingBlocks/Resilience/Resilience.Http/Resilience.Http.csproj src/BuildingBlocks/Resilience/Resilience.Http/
COPY src/Services/Ordering/Ordering.Infrastructure/Ordering.Infrastructure.csproj src/Services/Ordering/Ordering.Infrastructure/
COPY src/Services/Ordering/Ordering.Domain/Ordering.Domain.csproj src/Services/Ordering/Ordering.Domain/
COPY src/Services/Ordering/Ordering.API/Ordering.API.csproj src/Services/Ordering/Ordering.API/
COPY src/Services/Basket/Basket.API/Basket.API.csproj src/Services/Basket/Basket.API/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/test/Services/UnitTest
RUN dotnet build -c Release 
ENTRYPOINT ["dotnet", "test", "-c","Release", "--logger:trx"]
