$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path
 
Write-Host "Current script directory is $scriptPath" -ForegroundColor Yellow

#*** Basket service image ***
$basketPathToJson = $scriptPath + "\src\Services\Basket\Basket.API\project.json"
Write-Host "basketPathToJson is $basketPathToJson" -ForegroundColor Yellow
$basketPathToPub = $scriptPath + "\pub\basket"
Write-Host "basketPathToPub is $basketPathToPub" -ForegroundColor Yellow

Write-Host "Restore Dependencies just in case as it is needed to run dotnet publish" -ForegroundColor Blue
dotnet restore $basketPathToJson
dotnet build $basketPathToJson
dotnet publish $basketPathToJson -o $basketPathToPub

docker build -t eshop/basket.api $basketPathToPub