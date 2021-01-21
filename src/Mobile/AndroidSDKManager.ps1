$AndroidToolPath = "${env:ProgramFiles(x86)}\Android\android-sdk\tools\android.bat"
if (!(Test-Path $AndroidToolPath)) {
  $AndroidToolPath = "$env:localappdata\Android\android-sdk\tools\android.bat"
} elseif (!(Test-Path $AndroidToolPath)) {
  Write-Error "Unable to find Android SDK Manager tools."
  return
}

Function Get-AndroidSDKs() {
    $output = & $AndroidToolPath list sdk --all
    $sdks = $output |% {
        if ($_ -match '(?<index>\d+)- (?<sdk>.+), revision (?<revision>[\d\.]+)') {
            $sdk = New-Object PSObject
            Add-Member -InputObject $sdk -MemberType NoteProperty -Name Index -Value $Matches.index
            Add-Member -InputObject $sdk -MemberType NoteProperty -Name Name -Value $Matches.sdk
            Add-Member -InputObject $sdk -MemberType NoteProperty -Name Revision -Value $Matches.revision
            $sdk
        }
    }
    $sdks
}

Function Install-AndroidSDK() {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory=$true, Position=0)]
        [PSObject[]]$sdks
    )

    $sdkIndexes = $sdks |% { $_.Index }
    $sdkIndexArgument = [string]::Join(',',  $sdkIndexes)
    Echo 'y' | & $AndroidToolPath update sdk -u -a -t $sdkIndexArgument
}

# Example usage:
# $sdks = Get-AndroidSDKs |? { $_.name -like 'sdk platform*API 10*' -or $_.name -like 'google apis*api 10' }
# Install-AndroidSDK -sdks $sdks

# https://github.com/AArnott