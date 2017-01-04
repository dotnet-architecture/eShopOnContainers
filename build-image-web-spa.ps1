$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path
 
Write-Host "Current script directory is $scriptPath" -ForegroundColor Yellow

$pubFolderToDelete = $scriptPath + "\pub"
remove-item -path $pubFolderToDelete -Force -Recurse -ErrorAction SilentlyContinue

# *** WebSPA image ***
$webSPAPathToJson = $scriptPath + "\src\Web\WebSPA\eShopOnContainers.WebSPA\project.json"
Write-Host "webSPAPathToJson is $webSPAPathToJson" -ForegroundColor Yellow
$webSPAPathToPub = $scriptPath + "\pub\webSPA"
$webSPAPathToNpmBat = $scriptPath + "\src\Web\WebSPA\eShopOnContainers.WebSPA\buildspa.bat"
Write-Host "webSPAPathToPub is $webSPAPathToPub" -ForegroundColor Yellow

Write-Host "Restore Dependencies just in case as it is needed to run dotnet publish" -ForegroundColor Blue
dotnet restore $webSPAPathToJson
dotnet build $webSPAPathToJson
# Start-Process "cmd.exe" "/c " + $webSPAPathToNpmBat
dotnet publish $webSPAPathToJson -o $webSPAPathToPub

# *** identitySvc image ***
$identitySvcPathToJson = $scriptPath + "\src\Services\Identity\eShopOnContainers.Identity\project.json"
Write-Host "identitySvcPathToJson is $identitySvcPathToJson" -ForegroundColor Yellow
$identitySvcPathToPub = $scriptPath + "\pub\identity"
Write-Host "identitySvcPathToPub is $identitySvcPathToPub" -ForegroundColor Yellow

Write-Host "Restore Dependencies just in case as it is needed to run dotnet publish" -ForegroundColor Blue
dotnet restore $identitySvcPathToJson
dotnet build $identitySvcPathToJson
dotnet publish $identitySvcPathToJson -o $identitySvcPathToPub


#*** Catalog service image ***
$catalogPathToJson = $scriptPath + "\src\Services\Catalog\Catalog.API\project.json"
Write-Host "catalogPathToJson is $catalogPathToJson" -ForegroundColor Yellow
$catalogPathToPub = $scriptPath + "\pub\catalog"
Write-Host "catalogPathToPub is $catalogPathToPub" -ForegroundColor Yellow

Write-Host "Restore Dependencies just in case as it is needed to run dotnet publish" -ForegroundColor Blue
dotnet restore $catalogPathToJson
dotnet build $catalogPathToJson
dotnet publish $catalogPathToJson -o $catalogPathToPub

#*** Ordering service image ***
$orderingPathToJson = $scriptPath + "\src\Services\Ordering\Ordering.API\project.json"
Write-Host "orderingPathToJson is $orderingPathToJson" -ForegroundColor Yellow
$orderingPathToPub = $scriptPath + "\pub\ordering"
Write-Host "orderingPathToPub is $orderingPathToPub" -ForegroundColor Yellow

Write-Host "Restore Dependencies just in case as it is needed to run dotnet publish" -ForegroundColor Blue
dotnet restore $orderingPathToJson
dotnet build $orderingPathToJson
dotnet publish $orderingPathToJson -o $orderingPathToPub

#*** Basket service image ***
$basketPathToJson = $scriptPath + "\src\Services\Basket\Basket.API\project.json"
Write-Host "basketPathToJson is $basketPathToJson" -ForegroundColor Yellow
$basketPathToPub = $scriptPath + "\pub\basket"
Write-Host "basketPathToPub is $basketPathToPub" -ForegroundColor Yellow

Write-Host "Restore Dependencies just in case as it is needed to run dotnet publish" -ForegroundColor Blue
dotnet restore $basketPathToJson
dotnet build $basketPathToJson
dotnet publish $basketPathToJson -o $basketPathToPub

#!/bin/bash
# Delete all containers
docker rm $(docker ps -a -q) -f
# Delete all images
docker rmi $(docker images -q)

#*** build docker images ***
docker build -t eshop/catalog.api $catalogPathToPub
docker build -t eshop/ordering.api $orderingPathToPub
docker build -t eshop/basket.api $basketPathToPub
docker build -t eshop/webspa $webSPAPathToPub
docker build -t eshop/identity $identitySvcPathToPub