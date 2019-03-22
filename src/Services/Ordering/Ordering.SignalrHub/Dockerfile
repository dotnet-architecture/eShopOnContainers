FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY . .
WORKDIR /src/src/Services/Ordering/Ordering.SignalrHub
RUN dotnet restore -nowarn:msb3202,nu1503
RUN dotnet build  --no-restore  Ordering.SignalrHub.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish  --no-restore  Ordering.SignalrHub.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Ordering.SignalrHub.dll"]
