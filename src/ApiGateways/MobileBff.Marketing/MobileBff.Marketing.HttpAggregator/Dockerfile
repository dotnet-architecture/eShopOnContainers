FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY *.sln ./
COPY src/ApiGateways/MobileBff.Marketing/MobileBff.Marketing.HttpAggregator/MobileBff.Marketing.HttpAggregator.csproj src/ApiGateways/MobileBff.Marketing/MobileBff.Marketing.HttpAggregator/
RUN dotnet restore
COPY . .
WORKDIR /src/src/ApiGateways/MobileBff.Marketing/MobileBff.Marketing.HttpAggregator
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MobileBff.Marketing.HttpAggregator.dll"]
