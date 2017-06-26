#!/bin/bash

declare -x path=$1

if [ -z "$path" ]; then 
    $path="$(pwd)/../src"; 
fi

declare -a projectList=(
    "$path/Web/WebSPA"
    "$path/Services/Catalog/Catalog.API"
    "$path/Services/Basket/Basket.API"
    "$path/Services/Ordering/Ordering.API"
    "$path/Services/Identity/Identity.API"
    "$path/Services/Location/Locations.API"
    "$path/Services/Marketing/Marketing.API"
    "$path/Services/Payment/Payment.API"
    "$path/Services/GracePeriod/GracePeriodManager"
    "$path/Web/WebMVC"
    "$path/Web/WebStatus"
)

# Build SPA app
# pushd $(pwd)../src/Web/WebSPA
# npm run build:prod

for project in "${projectList[@]}"
do
    echo -e "\e[33mWorking on $path/$project"
    echo -e "\e[33m\tRemoving old publish output"
    pushd $path/$project
    rm -rf obj/Docker/publish
    echo -e "\e[33m\tRestoring project"
    dotnet restore
    echo -e "\e[33m\tBuilding and publishing projects"
    dotnet publish -o obj/Docker/publish
    popd
done

## remove old docker images:
#images=$(docker images --filter=reference="eshop/*" -q)
#if [ -n "$images" ]; then
#    docker rm $(docker ps -a -q) -f
#    echo "Deleting eShop images in local Docker repo"
#    echo $images
#    docker rmi $(docker images --filter=reference="eshop/*" -q) -f
#fi


# No need to build the images, docker build or docker compose will
# do that using the images and containers defined in the docker-compose.yml file.
