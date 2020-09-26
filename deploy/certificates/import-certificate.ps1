param (
    [Parameter(Mandatory = $true)]
    [string]$Password
)

$CertPassword = ConvertTo-SecureString -String "$Password" -Force -AsPlainText

Import-PfxCertificate -Exportable -FilePath .\docker-self-signed.pfx -CertStoreLocation Cert:\CurrentUser\Root\ -Password $CertPassword
