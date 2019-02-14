FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
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