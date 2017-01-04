$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path
 
Write-Host "Current script directory is $scriptPath" -ForegroundColor Yellow

#*** Ordering service image ***
$orderingPathToJson = $scriptPath + "\src\Services\Ordering\Ordering.API\project.json"
Write-Host "orderingPathToJson is $orderingPathToJson" -ForegroundColor Yellow
$orderingPathToPub = $scriptPath + "\pub\ordering"
Write-Host "orderingPathToPub is $orderingPathToPub" -ForegroundColor Yellow

Write-Host "Restore Dependencies just in case as it is needed to run dotnet publish" -ForegroundColor Blue
dotnet restore $orderingPathToJson
dotnet build $orderingPathToJson
dotnet publish $orderingPathToJson -o $orderingPathToPub

docker build -t eshop/ordering.api $orderingPathToPub