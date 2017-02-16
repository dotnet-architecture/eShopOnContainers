#!/bin/sh
dotnet restore
rm -rf ./pub
dotnet publish "$(pwd)/src/Web/WebMVC/project.json" -o "$(pwd)/pub/webMVC"
dotnet publish "$(pwd)/src/Services/Catalog/Catalog.API/project.json" -o "$(pwd)/pub/catalog"
dotnet publish "$(pwd)/src/Services/Ordering/Ordering.API/project.json" -o "$(pwd)/pub/ordering"
dotnet publish "$(pwd)/src/Services/Basket/Basket.API/project.json" -o "$(pwd)/pub/basket"

pushd "$(pwd)/src/Web/WebSPA/eShopOnContainers.WebSPA"
npm install
npm run build:prod
popd

dotnet publish "$(pwd)/src/Web/WebSPA/eShopOnContainers.WebSPA/project.json" -o "$(pwd)/pub/webSPA"

dotnet publish "$(pwd)/src/Services/Identity/Identity.API/project.json" -o "$(pwd)/pub/identity"

docker build -t eshop/web "$(pwd)/pub/webMVC"
docker build -t eshop/catalog.api "$(pwd)/pub/catalog"
docker build -t eshop/ordering.api "$(pwd)/pub/ordering"
docker build -t eshop/basket.api "$(pwd)/pub/basket"
docker build -t eshop/webspa "$(pwd)/pub/webSPA"
docker build -t eshop/identity "$(pwd)/pub/identity"

