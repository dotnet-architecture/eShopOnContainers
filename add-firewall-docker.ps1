param([switch]$Elevated)
function Check-Admin {
$currentUser = New-Object Security.Principal.WindowsPrincipal $([Security.Principal.WindowsIdentity]::GetCurrent())
$currentUser.IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)
}
if ((Check-Admin) -eq $false)  {
if ($elevated)
{
# could not elevate, quit
}
 
else {
 
Start-Process powershell.exe -Verb RunAs -ArgumentList ('-noprofile -noexit -file "{0}" -elevated' -f ($myinvocation.MyCommand.Definition))
}
exit
}

try {
  Get-NetFirewallRule -DisplayName EshopDocker -ErrorAction Stop
  Write-Host "Rule found"
}
  catch [Exception] {
  New-NetFirewallRule -DisplayName EshopDocker -Confirm -Description "Eshop on Containers" -LocalAddress Any -LocalPort Any -Protocol tcp -RemoteAddress Any -RemotePort 5100-5105 -Direction Inbound
  New-NetFirewallRule -DisplayName EshopDocker -Confirm -Description "Eshop on Containers" -LocalAddress Any -LocalPort Any -Protocol tcp -RemoteAddress Any -RemotePort 5100-5105 -Direction Outbound
  ac -Encoding UTF8  C:\Windows\system32\drivers\etc\hosts "127.0.0.1 eshopcontainers" #avoid cors problem in Angular2-http-localhost
}