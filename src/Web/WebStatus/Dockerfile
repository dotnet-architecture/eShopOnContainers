FROM microsoft/dotnet:2.1.3-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1.401-sdk AS build
WORKDIR /src
COPY . .
WORKDIR /src/src/Web/WebStatus
RUN dotnet restore -nowarn:msb3202,nu1503
COPY src/Web/WebMVC/synergydev.pfx /root/.aspnet/https/
RUN dotnet build --no-restore -c Release -o /app

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebStatus.dll"]

