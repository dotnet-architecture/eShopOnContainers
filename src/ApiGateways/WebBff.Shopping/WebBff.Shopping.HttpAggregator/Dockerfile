FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY *.sln ./
COPY src/ApiGateways/WebBff.Shopping/WebBff.Shopping.HttpAggregator/WebBff.Shopping.HttpAggregator.csproj src/ApiGateways/WebBff.Shopping/WebBff.Shopping.HttpAggregator/
RUN dotnet restore
COPY . .
WORKDIR /src/src/ApiGateways/WebBff.Shopping/WebBff.Shopping.HttpAggregator
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebBff.Shopping.HttpAggregator.dll"]
