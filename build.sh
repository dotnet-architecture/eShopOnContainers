#!/bin/sh
#dotnet restore
rm -rf ./pub
dotnet publish "$(pwd)/src/Services/Catalog/Catalog.API/project.json" -o "$(pwd)/pub/catalog"
dotnet publish "$(pwd)/src/Web/Microsoft.eShopOnContainers.WebMVC/project.json" -o "$(pwd)/pub/webMVC"

docker build -t eshop/web "$(pwd)/pub/webMVC"
docker build -t eshop/catalog.api "$(pwd)/pub/catalog"