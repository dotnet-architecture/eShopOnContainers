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
$reglas = Get-NetFirewallRule -DisplayName 'EshopDocker'
if ($reglas.Length -gt 0)
{
    New-NetFirewallRule -DisplayName EshopDocker -Confirm -Description "Eshop on Containers" -LocalAddress Any -LocalPort Any -Protocol tcp -RemoteAddress Any -RemotePort 5100-5105 -Direction Inbound
    New-NetFirewallRule -DisplayName EshopDocker -Confirm -Description "Eshop on Containers" -LocalAddress Any -LocalPort Any -Protocol tcp -RemoteAddress Any -RemotePort 5100-5105 -Direction Outbound
}