FROM microsoft/dotnet:2.2.0-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2.100-sdk AS build
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
