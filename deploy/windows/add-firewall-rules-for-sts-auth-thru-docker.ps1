param(
  [string]$Name = "eShopOnContainers",
  [string]$InboundDisplayName = "eShopOnContainers-Inbound",
  [string]$OutboundDisplayName = "eShopOnContainers-Outbound",
  [switch]$Elevated
  )

function Check-Admin {
  $currentUser = New-Object Security.Principal.WindowsPrincipal $([Security.Principal.WindowsIdentity]::GetCurrent())
  $currentUser.IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)
}
function Add-InboundRule {
  New-NetFirewallRule -DisplayName $InboundDisplayName -Confirm -Description "$Name Inbound Rule for port range 5100-5150" -LocalAddress Any -LocalPort 5100-5150 -Protocol tcp -RemoteAddress Any -RemotePort Any -Direction Inbound
}
function Add-OutboundRule {
  New-NetFirewallRule -DisplayName $OutboundDisplayName -Confirm -Description "$Name Outbound Rule for port range 5100-5150" -LocalAddress Any -LocalPort 5100-5150 -Protocol tcp -RemoteAddress Any -RemotePort Any -Direction Outbound
}

if ((Check-Admin) -eq $false) {
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
  $rules = $(Get-NetFirewallRule -DisplayName $Name-* -ErrorAction Stop | Out-String)
  if (!$rules.Contains($InboundDisplayName) -and !$rules.Contains($OutboundDisplayName))
  {
    Add-InboundRule
    Add-OutboundRule
  } 
  elseif (!$rules.Contains($InboundDisplayName))
  {
    Add-InboundRule
  }  
  elseif (!$rules.Contains($OutboundDisplayName))
  {
    Add-OutboundRule
  }
  else{
    Write-Host "Rules found!"
  }
}
catch [Exception] {
  Add-InboundRule
  Add-OutboundRule
}
