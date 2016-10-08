
$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path
 
Write-Host "Current script directory is $scriptPath" -ForegroundColor Yellow

$pubFolderToDelete = $scriptPath + "\pub"
remove-item -path $pubFolderToDelete -Force -Recurse -ErrorAction SilentlyContinue
#cmd /c "rd /s pub" /q


$webPathToJson = $scriptPath + "\src\Web\WebMVC\project.json"
Write-Host "webPathToJson is $webPathToJson" -ForegroundColor Yellow
$webPathToPub = $scriptPath + "\pub\webMVC"
Write-Host "webPathToPub is $webPathToPub" -ForegroundColor Yellow
dotnet publish $webPathToJson -o $webPathToPub

$catalogPathToJson = $scriptPath + "\src\Services\Catalog\Catalog.API\project.json"
Write-Host "catalogPathToJson is $catalogPathToJson" -ForegroundColor Yellow
$catalogPathToPub = $scriptPath + "\pub\catalog"
Write-Host "catalogPathToPub is $catalogPathToPub" -ForegroundColor Yellow
dotnet publish $catalogPathToJson -o $catalogPathToPub

$orderingPathToJson = $scriptPath + "\src\Services\Ordering\Ordering.API\project.json"
Write-Host "orderingPathToJson is $orderingPathToJson" -ForegroundColor Yellow
$orderingPathToPub = $scriptPath + "\pub\ordering"
Write-Host "orderingPathToPub is $orderingPathToPub" -ForegroundColor Yellow
dotnet publish $orderingPathToJson -o $orderingPathToPub
 

docker build -t eshop/web $webPathToPub
docker build -t eshop/catalog.api $catalogPathToPub
docker build -t eshop/ordering.api $orderingPathToPub