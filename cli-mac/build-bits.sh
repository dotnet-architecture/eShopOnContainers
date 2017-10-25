#!/bin/sh

# List of microservices here needs to be updated to include all the new microservices (Marketing, etc.)

projectList=(
    "../src/Web/WebMVC"
    "../src/Web/WebSPA"
    "../src/Services/Identity/Identity.API"
    "../src/Services/Catalog/Catalog.API"
    "../src/Services/Ordering/Ordering.API"
    "../src/Services/Basket/Basket.API"
    "../src/Services/Location/Locations.API"
    "../src/Services/Marketing/Marketing.API"
    "../src/Services/Payment/Payment.API"
    "../src/Web/WebStatus"
)


pushd $(pwd)/../src/Web/WebSPA
npm install
npm rebuild node-sass
popd 

for project in "${projectList[@]}"
do
    echo -e "\e[33mWorking on $(pwd)/$project"
    echo -e "\e[33m\tRemoving old publish output"
    pushd $(pwd)/$project
    rm -rf obj/Docker/publish
    echo -e "\e[33m\tBuilding and publishing projects"
    dotnet publish -o obj/Docker/publish -c Release
    popd
done

# remove old docker images:
images=$(docker images --filter=reference="eshop/*" -q)
if [ -n "$images" ]; then
    docker rm $(docker ps -a -q) -f
    echo "Deleting eShop images in local Docker repo"
    echo $images
    docker rmi $(docker images --filter=reference="eshop/*" -q) -f
fi

# No need to build the images, docker build or docker compose will
# do that using the images and containers defined in the docker-compose.yml file.
#
#
