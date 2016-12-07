$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path
 
Write-Host "Current script directory is $scriptPath" -ForegroundColor Yellow

# *** identitySvc image ***
$identitySvcPathToJson = $scriptPath + "\src\Services\Identity\eShopOnContainers.Identity\project.json"
Write-Host "identitySvcPathToJson is $identitySvcPathToJson" -ForegroundColor Yellow
$identitySvcPathToPub = $scriptPath + "\pub\identity"
Write-Host "identitySvcPathToPub is $identitySvcPathToPub" -ForegroundColor Yellow

Write-Host "Restore Dependencies just in case as it is needed to run dotnet publish" -ForegroundColor Blue
dotnet restore $identitySvcPathToJson
dotnet build $identitySvcPathToJson
dotnet publish $identitySvcPathToJson -o $identitySvcPathToPub

docker build -t eshop/identity $identitySvcPathToPub