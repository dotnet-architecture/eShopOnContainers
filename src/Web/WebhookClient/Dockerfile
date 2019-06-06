FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ["src/Web/WebhookClient/WebhookClient.csproj", "src/Web/WebhookClient/"]
RUN dotnet restore "src/Web/WebhookClient/WebhookClient.csproj"
COPY . .
WORKDIR "/src/src/Web/WebhookClient"
RUN dotnet build "WebhookClient.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WebhookClient.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebhookClient.dll"]