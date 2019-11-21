param (
  [string]$solution = "eShopOnContainers-ServicesAndWebApps.sln"
)

$outfile = "DockerfileSolutionRestore.txt"

# This script creates the $outfile file, with Dockerfile commands to restore all the packages for the solution,
# so you can insert them (by hand) into Dockerfiles right before the "COPY . ." line
# to increase build speed by optimizing the use of docker build images cache.

# This script is only needed when adding or removing projects from the solution.

Write-Output "COPY ""$solution"" ""$solution""" > $outfile 

Add-Content -Path $outfile ""
Select-String -Path $solution -Pattern ', "(.*?\.csproj)"' | ForEach-Object { $_.Matches.Groups[1].Value.Replace("\", "/") } | Sort-Object | ForEach-Object {"COPY ""$_"" ""$_"""} | Out-File -FilePath $outfile -Append
Add-Content -Path $outfile ""
Select-String -Path $solution -Pattern ', "(.*?\.dcproj)"' | ForEach-Object { $_.Matches.Groups[1].Value.Replace("\", "/") } | Sort-Object | ForEach-Object {"COPY ""$_"" ""$_"""} | Out-File -FilePath $outfile -Append
Add-Content -Path $outfile ""
Add-Content -Path $outfile "COPY ""NuGet.config"" ""NuGet.config"""
Add-Content -Path $outfile ""
Add-Content -Path $outfile "RUN dotnet restore ""$solution"""

Get-Content $outfile
