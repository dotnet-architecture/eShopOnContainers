FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src

COPY scripts scripts/

COPY ApiGateways/*/*/*.csproj csproj-files/
COPY BuildingBlocks/*/*/*.csproj csproj-files/
COPY Services/*/*/*.csproj csproj-files/
COPY Web/*/*.csproj csproj-files/

COPY . .
WORKDIR /src/ApiGateways/Web.Bff.Shopping/aggregator
RUN dotnet publish -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Web.Shopping.HttpAggregator.dll"]
