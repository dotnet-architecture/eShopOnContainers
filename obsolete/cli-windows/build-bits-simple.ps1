# This approach still has issues, but would be the simplest approach for this script
# See: https://github.com/dotnet/eShopOnContainers/issues/74 

Param([string] $rootPath)
$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path

Write-Host "Current script directory is $scriptPath" -ForegroundColor Yellow

if ([string]::IsNullOrEmpty($rootPath)) {
    $rootPath = "$scriptPath\.."
}
Write-Host "Root path used is $rootPath" -ForegroundColor Yellow

$SolutionFilePath = [IO.Path]::Combine($rootPath, "eShopOnContainers-ServicesAndWebApps.sln")

dotnet publish $SolutionFilePath -c Release -o .\obj\Docker\publish

