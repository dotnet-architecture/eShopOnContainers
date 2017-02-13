$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path
 
Write-Host "Current script directory is $scriptPath" -ForegroundColor Yellow

$pubFolderToDelete = $scriptPath + "..\..\pub"
remove-item -path $pubFolderToDelete -Force -Recurse -ErrorAction SilentlyContinue
#cmd /c "rd /s pub" /q

# *** WebMVC image ***
$webPathToJson = $scriptPath + "\src\Web\WebMVC\project.json"
Write-Host "webPathToJson is $webPathToJson" -ForegroundColor Yellow
$webPathToPub = $scriptPath + "\pub\webMVC"
Write-Host "webPathToPub is $webPathToPub" -ForegroundColor Yellow

Write-Host "Restore Dependencies just in case as it is needed to run dotnet publish" -ForegroundColor Blue
dotnet restore $webPathToJson
dotnet build $webPathToJson
dotnet publish $webPathToJson -o $webPathToPub

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
$orderingPath = $scriptPath + "\src\Services\Ordering"
Write-Host "orderingPath is $orderingPath" -ForegroundColor Yellow
$orderingApiPathToJson = $orderingPath + "\Ordering.API\project.json"
Write-Host "orderingApiPathToJson is $orderingApiPathToJson" -ForegroundColor Yellow
$orderingApiPathToPub = $scriptPath + "\pub\ordering"
Write-Host "orderingApiPathToPub is $orderingApiPathToPub" -ForegroundColor Yellow

Write-Host "Restore Dependencies just in case as it is needed to run dotnet publish" -ForegroundColor Blue
dotnet restore $orderingPath
dotnet build $orderingApiPathToJson
dotnet publish $orderingApiPathToJson -o $orderingApiPathToPub

#*** Basket service image ***
$basketPathToJson = $scriptPath + "\src\Services\Basket\Basket.API\project.json"
Write-Host "basketPathToJson is $basketPathToJson" -ForegroundColor Yellow
$basketPathToPub = $scriptPath + "\pub\basket"
Write-Host "basketPathToPub is $basketPathToPub" -ForegroundColor Yellow

Write-Host "Restore Dependencies just in case as it is needed to run dotnet publish" -ForegroundColor Blue
dotnet restore $basketPathToJson
dotnet build $basketPathToJson
dotnet publish $basketPathToJson -o $basketPathToPub

# Delete all eshop containers
docker rm $(docker images --filter=reference="eshop/*" -q) -f
# Delete all eshop images
docker rmi $(docker images --filter=reference="eshop/*" -q)

#*** build docker images ***
docker build -t eshop/web $webPathToPub
docker build -t eshop/catalog.api $catalogPathToPub
docker build -t eshop/ordering.api $orderingPathToPub
docker build -t eshop/basket.api $basketPathToPub
docker build -t eshop/identity $identitySvcPathToPub