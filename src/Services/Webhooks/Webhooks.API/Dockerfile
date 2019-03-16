FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["src/Services/Webhooks/Webhooks.API/Webhooks.API.csproj", "src/Services/Webhooks/Webhooks.API/"]
RUN dotnet restore "src/Services/Webhooks/Webhooks.API/Webhooks.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Webhooks/Webhooks.API"
RUN dotnet build "Webhooks.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Webhooks.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Webhooks.API.dll"]