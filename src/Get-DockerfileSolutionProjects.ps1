param (
  [string]$solution = "eShopOnContainers-ServicesAndWebApps.sln"
)

$outfile = "DockerfileSolutionProjects.txt"

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
