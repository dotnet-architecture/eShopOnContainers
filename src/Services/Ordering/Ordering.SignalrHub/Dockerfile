FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY eShopOnContainers-ServicesAndWebApps.sln ./
COPY src/Services/Ordering/Ordering.SignalrHub/Ordering.SignalrHub.csproj src/Services/Ordering/Ordering.SignalrHub/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/src/Services/Ordering/Ordering.SignalrHub
RUN dotnet build Ordering.SignalrHub.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Ordering.SignalrHub.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Ordering.SignalrHub.dll"]
