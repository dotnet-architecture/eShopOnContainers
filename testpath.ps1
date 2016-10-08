#$scriptPath = [System.IO.Path]::GetDirectoryName($myInvocation.MyCommand.Definition)

$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path
 
Write-Host "Current script directory is $scriptPath"