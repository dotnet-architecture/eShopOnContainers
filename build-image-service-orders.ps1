$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path
 
Write-Host "Current script directory is $scriptPath" -ForegroundColor Yellow

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

docker build -t eshop/ordering.api $orderingPathToPub