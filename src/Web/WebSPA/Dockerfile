ARG NODE_IMAGE=node:8.11
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM ${NODE_IMAGE} as node-build
WORKDIR /web
COPY src/Web/WebSPA .
RUN npm install
RUN npm run build:prod

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src

COPY scripts scripts/

COPY src/ApiGateways/*/*.csproj /src/csproj-files/
COPY src/ApiGateways/*/*/*.csproj /src/csproj-files/
COPY src/BuildingBlocks/*/*/*.csproj /src/csproj-files/
COPY src/Services/*/*/*.csproj /src/csproj-files/
COPY src/Web/*/*.csproj /src/csproj-files/

COPY . .
COPY --from=node-build /web/wwwroot /src/src/Web/WebSPA/wwwroot/
WORKDIR /src/src/Web/WebSPA
RUN dotnet publish -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebSPA.dll"]
