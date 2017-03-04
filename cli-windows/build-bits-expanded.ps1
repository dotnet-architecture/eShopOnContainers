
#########################################################################################################
# This "expanded Script" can be used when debugging issues when building the .NET Core bits 
# as it is easier to follow and debug than when using a loop (like in the optimized build-bits.ps1)
#########################################################################################################

Param([string] $rootPath)
$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path

Write-Host "Current script directory is $scriptPath" -ForegroundColor Yellow

if ([string]::IsNullOrEmpty($rootPath)) {
    $rootPath = "$scriptPath\.."
}
Write-Host "Root path used is $rootPath" -ForegroundColor Yellow

# *** WebMVC paths ***
$webMVCPath = $rootPath + "\src\Web\WebMVC"
$webMVCPathToProject = $webMVCPath + "\WebMVC.csproj"
Write-Host "webMVCPathToProject is $webMVCPathToProject" -ForegroundColor Yellow
$webMVCPathToPub = $webMVCPath + "\obj\Docker\publish"
Write-Host "webMVCPathToPub is $webMVCPathToPub" -ForegroundColor Yellow


# *** WebSPA paths ***
$webSPAPath = $rootPath + "\src\Web\WebSPA"
$webSPAPathToProject = $webSPAPath + "\WebSPA.csproj"
Write-Host "webSPAPathToProject is $webSPAPathToProject" -ForegroundColor Yellow
$webSPAPathToPub = $webSPAPath + "\obj\Docker\publish"
Write-Host "webSPAPathToPub is $webSPAPathToPub" -ForegroundColor Yellow


# *** IdentitySvc paths ***
$identitySvcPath = $rootPath + "\src\Services\Identity\Identity.API"
$identitySvcToProject = $identitySvcPath + "\Identity.API.csproj"
Write-Host "identitySvcToProject is $identitySvcToProject" -ForegroundColor Yellow
$identitySvcPathToPub = $identitySvcPath + "\obj\Docker\publish"
Write-Host "identitySvcPathToPub is $identitySvcPathToPub" -ForegroundColor Yellow


# *** Catalog paths ***
$catalogPath = $rootPath + "\src\Services\Catalog\Catalog.API"
$catalogPathToProject = $catalogPath + "\Catalog.API.csproj"
Write-Host "catalogPathToProject is $catalogPathToProject" -ForegroundColor Yellow
$catalogPathToPub = $catalogPath + "\obj\Docker\publish"
Write-Host "catalogPathToPub is $catalogPathToPub" -ForegroundColor Yellow


# *** Ordering paths ***
$orderingPath = $rootPath + "\src\Services\Ordering\Ordering.API"
$orderingPathToProject = $orderingPath + "\Ordering.API.csproj"
Write-Host "orderingPathToProject is $orderingPathToProject" -ForegroundColor Yellow
$orderingPathToPub = $orderingPath + "\obj\Docker\publish"
Write-Host "orderingPathToPub is $orderingPathToPub" -ForegroundColor Yellow

# *** Basket paths ***
$basketPath = $rootPath + "\src\Services\Basket\Basket.API"
$basketPathToProject = $basketPath + "\Basket.API.csproj"
Write-Host "basketPathToProject is $basketPathToProject" -ForegroundColor Yellow
$basketPathToPub = $basketPath + "\obj\Docker\publish"
Write-Host "basketPathToPub is $basketPathToPub" -ForegroundColor Yellow


########################################################################################
# Delete old eShop dotnet publish bits
########################################################################################
# Write-Host "Deleting previous dotnet publish bits from all projects" -ForegroundColor Blue

remove-item -path $WebMVCPathToPub -Force -Recurse -ErrorAction SilentlyContinue
remove-item -path $webSPAPathToPub -Force -Recurse -ErrorAction SilentlyContinue
remove-item -path $identitySvcPathToPub -Force -Recurse -ErrorAction SilentlyContinue
remove-item -path $catalogPathToPub -Force -Recurse -ErrorAction SilentlyContinue
remove-item -path $orderingPathToPub -Force -Recurse -ErrorAction SilentlyContinue
remove-item -path $basketPathToPub -Force -Recurse -ErrorAction SilentlyContinue



########################################################################################
# Building DotNet bits
########################################################################################

# WebMVC: Build dotnet bits
Write-Host "WebMVC: Restore Dependencies, dotnet build and dotnet publish" -ForegroundColor Blue
dotnet restore $WebMVCPathToProject
dotnet build $WebMVCPathToProject
dotnet publish $WebMVCPathToProject -o $WebMVCPathToPub


# WebSPA: Build dotnet bits
Write-Host "WebSPA: Installing npm dependencies"
#TEMP COMMENT---     Start-Process -WorkingDirectory $webSPAPath -NoNewWindow -Wait npm i

Write-Host "WebSPA: Restore Dependencies, dotnet build and dotnet publish" -ForegroundColor Blue
dotnet restore $webSPAPathToProject
dotnet build $webSPAPathToProject
dotnet publish $webSPAPathToProject -o $webSPAPathToPub


# Identity Service: Build dotnet bits
Write-Host "Identity Service: Restore Dependencies, dotnet build and dotnet publish" -ForegroundColor Blue
dotnet restore $identitySvcToProject
dotnet build $identitySvcToProject
dotnet publish $identitySvcToProject -o $identitySvcPathToPub


# Catalog Service: Build dotnet bits
Write-Host "Catalog Service: Restore Dependencies, dotnet build and dotnet publish" -ForegroundColor Blue
dotnet restore $catalogPathToProject
dotnet build $catalogPathToProject
dotnet publish $catalogPathToProject -o $catalogPathToPub


# Ordering Service: Build dotnet bits
Write-Host "Ordering Service: Restore Dependencies, dotnet build and dotnet publish" -ForegroundColor Blue
dotnet restore $orderingPathToProject
dotnet build $orderingPathToProject
dotnet publish $orderingPathToProject -o $orderingPathToPub


# Basket Service: Build dotnet bits
Write-Host "Basket Service: Restore Dependencies, dotnet build and dotnet publish" -ForegroundColor Blue
dotnet restore $basketPathToProject
dotnet build $basketPathToProject
dotnet publish $basketPathToProject -o $basketPathToPub



########################################################################################
# Delete old eShop Docker images
########################################################################################

$imagesToDelete = docker images --filter=reference="eshop/*" -q

If (-Not $imagesToDelete) {Write-Host "Not deleting eShop images as there are no eShop images in the current local Docker repo."} 
Else 
{
    # Delete all containers
    Write-Host "Deleting all containers in local Docker Host"
    docker rm $(docker ps -a -q) -f
    
    # Delete all eshop images
    Write-Host "Deleting eShop images in local Docker repo"
    Write-Host $imagesToDelete
    docker rmi $(docker images --filter=reference="eshop/*" -q) -f
}


########################################################################################
# Build new eShop images
########################################################################################

# WE DON'T NEED DOCKER BUILD AS WE CAN RUN "DOCKER-COMPOSE BUILD" OR "DOCKER-COMPOSE UP" AND IT WILL BUILD ALL THE IMAGES IN THE .YML FOR US

#*** build docker images ***
# docker build -t eshop/web $webPathToPub
# docker build -t eshop/catalog.api $catalogPathToPub
# docker build -t eshop/ordering.api $orderingApiPathToPub
# docker build -t eshop/basket.api $basketPathToPub
# docker build -t eshop/webspa $webSPAPathToPub
# docker build -t eshop/identity $identitySvcPathToPub