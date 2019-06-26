FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src

COPY scripts scripts/

COPY src/ApiGateways/*/*.csproj /src/csproj-files/
COPY src/ApiGateways/*/*/*.csproj /src/csproj-files/
COPY src/BuildingBlocks/*/*/*.csproj /src/csproj-files/
COPY src/Services/*/*/*.csproj /src/csproj-files/
COPY src/Web/*/*.csproj /src/csproj-files/

COPY . .
WORKDIR /src/src/Services/Catalog/Catalog.API
RUN dotnet publish -c Release -o /app

FROM build as unittest
WORKDIR /src/src/Services/Catalog/Catalog.UnitTests

FROM build as functionaltest
WORKDIR /src/src/Services/Catalog/Catalog.FunctionalTests

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=build /src/src/Services/Catalog/Catalog.API/Proto /app/Proto
COPY --from=build /src/src/Services/Catalog/Catalog.API/eshop.pfx .
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
