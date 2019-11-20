 # Delete all containers
 Write-Host "Deleting all running containers in the local Docker Host"
 docker rm $(docker ps -a -q) -f

$eShopImagesToDelete = docker images --filter=reference="eshop/*" -q
If (-Not $eShopImagesToDelete) {Write-Host "Not deleting eShop images as there are no eShop images in the current local Docker repo."} 
Else 
{    
    # Delete all eshop images
    Write-Host "Deleting eShop images in local Docker repo"
    Write-Host $eShopImagesToDelete
    docker rmi $(docker images --filter=reference="eshop/*" -q) -f
}

$VSImagesToDelete = docker images --filter=reference="catalog.api:dev" -q
If (-Not $VSImagesToDelete) {Write-Host "Not deleting VS images as there are no VS images in the current local Docker repo."} 
Else 
{    
    # Delete all eshop images
    Write-Host "Deleting images created by VS in local Docker repo"
    Write-Host $VSImagesToDelete
    docker rmi $(docker images --filter=reference="*:dev" -q) -f
    
    #docker rmi $(docker images --filter=reference="eshop/payment.api:dev" -q) -f
    #docker rmi $(docker images --filter=reference="eshop/webspa:dev" -q) -f
    #docker rmi $(docker images --filter=reference="eshop/webmvc:dev" -q) -f
    #docker rmi $(docker images --filter=reference="eshop/catalog.api:dev" -q) -f
    #docker rmi $(docker images --filter=reference="eshop/marketing.api:dev" -q) -f
    #docker rmi $(docker images --filter=reference="eshop/ordering.api:dev" -q) -f
    #docker rmi $(docker images --filter=reference="eshop/basket.api:dev" -q) -f
    #docker rmi $(docker images --filter=reference="eshop/identity.api:dev" -q) -f
    #docker rmi $(docker images --filter=reference="eshop/locations.api:dev" -q) -f
    #docker rmi $(docker images --filter=reference="eshop/webstatus:dev" -q) -f
}

# DELETE ALL IMAGES AND CONTAINERS

# Delete all containers
# docker rm $(docker ps -a -q) -f

# Delete all images
# docker rmi $(docker images -q)

#Filter by image name (Has to be complete, cannot be a wildcard)
#docker ps -q  --filter=ancestor=eshop/identity.api:dev

