FROM microsoft/dotnet:1.0.0-core
ARG source=.
WORKDIR /app
ENV ASPNETCORE_URLS http://*:80
EXPOSE 80
COPY $source .
ENTRYPOINT dotnet Microsoft.eShopOnContainers.WebMVC.dll
