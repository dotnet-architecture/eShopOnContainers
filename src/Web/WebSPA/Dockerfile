ARG NODE_IMAGE=node:8.11
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM ${NODE_IMAGE} as node-build
WORKDIR /web
COPY Web/WebSPA .
RUN npm install
RUN npm run build:prod

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src

COPY scripts scripts/

COPY ApiGateways/*/*.csproj csproj-files/
COPY ApiGateways/*/*/*.csproj csproj-files/
COPY BuildingBlocks/*/*/*.csproj csproj-files/
COPY Services/*/*/*.csproj csproj-files/
COPY Web/*/*.csproj csproj-files/

COPY . .
COPY --from=node-build /web/wwwroot /src/Web/WebSPA/wwwroot/
WORKDIR /src/Web/WebSPA
RUN dotnet publish -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebSPA.dll"]
