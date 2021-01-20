 #Requires -RunAsAdministrator
 Get-NetConnectionProfile | Where-Object { $_.InterfaceAlias -match "(DockerNAT)" } | ForEach-Object { Set-NetConnectionProfile -InterfaceIndex $_.InterfaceIndex -NetworkCategory Private }
