FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src

COPY . .
WORKDIR /src/Services/Basket/Basket.API
RUN dotnet publish -c Release -o /app

FROM build as unittest
WORKDIR /src/Services/Basket/Basket.UnitTests

FROM build as functionaltest
WORKDIR /src/Services/Basket/Basket.FunctionalTests

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Basket.API.dll"]
