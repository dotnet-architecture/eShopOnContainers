ARG NODE_IMAGE=node:8.11
FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk as dotnet-build
WORKDIR /src

FROM ${NODE_IMAGE} as node-build
WORKDIR /web
COPY src/Web/WebMVC .
RUN npm install -g bower@1.8.4
RUN bower install --allow-root

FROM dotnet-build as build
WORKDIR /src/src/Web/WebMVC/wwwroot
COPY --from=node-build /web/wwwroot .
WORKDIR /src
COPY . .
WORKDIR /src/src/Web/WebMVC
RUN dotnet restore -nowarn:msb3202,nu1503

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebMVC.dll"]
