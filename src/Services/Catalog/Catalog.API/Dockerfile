FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src

COPY . .
WORKDIR /src/Services/Catalog/Catalog.API
RUN dotnet publish -c Release -o /app

FROM build as unittest
WORKDIR /src/Services/Catalog/Catalog.UnitTests

FROM build as functionaltest
WORKDIR /src/Services/Catalog/Catalog.FunctionalTests

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=build /src/Services/Catalog/Catalog.API/Proto /app/Proto
COPY --from=build /src/Services/Catalog/Catalog.API/eshop.pfx .
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
