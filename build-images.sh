#!/bin/sh
#dotnet restore
rm -rf ./pub
dotnet publish "$(pwd)/src/Web/WebMVC/project.json" -o "$(pwd)/pub/webMVC"
dotnet publish "$(pwd)/src/Services/Catalog/Catalog.API/project.json" -o "$(pwd)/pub/catalog"
dotnet publish "$(pwd)/src/Services/Ordering/Ordering.API/project.json" -o "$(pwd)/pub/ordering"
dotnet publish "$(pwd)/src/Services/Basket/Basket.API/project.json" -o "$(pwd)/pub/basket"

docker build -t eshop/web "$(pwd)/pub/webMVC"
docker build -t eshop/catalog.api "$(pwd)/pub/catalog"
docker build -t eshop/ordering.api "$(pwd)/pub/ordering"
docker build -t eshop/basket.api "$(pwd)/pub/basket"