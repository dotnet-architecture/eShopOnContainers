FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
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