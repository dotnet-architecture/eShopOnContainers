Param(
    [parameter(Mandatory=$true)][string]$vaultName,
    [parameter(Mandatory=$true)][string]$certName,
    [parameter(Mandatory=$true)][string]$certPwd,
    [parameter(Mandatory=$true)][string]$subjectName,
    [parameter(Mandatory=$false)][string]$ValidityInMonths=12,
    [parameter(Mandatory=$true)][string]$saveDir      
)


#Log in Azure Account
Login-AzureRmAccount


# Create Cert in KeyVault
Write-Host "Creating certificate in Azure KeyVault..." -ForegroundColor Yellow
$policy = New-AzureKeyVaultCertificatePolicy  -SubjectName $subjectName -IssuerName Self -ValidityInMonths $ValidityInMonths
Add-AzureKeyVaultCertificate -VaultName $vaultName -Name $certName -CertificatePolicy $policy

# Downloading Certificate
Write-Host "Downloading Certificate from KeyVault..." -ForegroundColor Yellow

$Stoploop = $false
$Retrycount = 0

do {
	try {

		$kvSecret = Get-AzureKeyVaultSecret -VaultName $vaultName -Name $certName -ErrorAction SilentlyContinue
        $kvSecretBytes = [System.Convert]::FromBase64String($kvSecret.SecretValueText)
        $certCollection = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2Collection
        $certCollection.Import($kvSecretBytes,$null,[System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::Exportable)
        $protectedCertificateBytes = $certCollection.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Pkcs12, $certPwd)
        [System.IO.File]::WriteAllBytes($saveDir + "\" + $certName + ".pfx", $protectedCertificateBytes)
        
        $Stoploop = $true
        Write-Host "Finished!" -ForegroundColor Yellow		
	}
	catch {
        if ($Retrycount -gt 5){
            $Stoploop = $true
            Write-Host "Not possible to retrieve the certificate!" -ForegroundColor Yellow
		}
		else {
			Start-Sleep -Seconds 20
			$Retrycount = $Retrycount + 1
		}
	}
}
While ($Stoploop -eq $false)

# Show Certificate Values
Get-AzureKeyVaultCertificate -VaultName $vaultName -Name $certName