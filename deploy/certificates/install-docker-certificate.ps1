param (
    [Parameter(Mandatory = $true)]
    [string]$Password
)

# Import into current user root CA store
$CertPassword = ConvertTo-SecureString -String "$Password" -Force -AsPlainText
Import-PfxCertificate -Exportable -FilePath .\docker-self-signed.pfx -CertStoreLocation Cert:\CurrentUser\Root\ -Password $CertPassword

# Copy to user profile to use as HTTPS certificate in server containers
mkdir $env:USERPROFILE\.aspnet\https -Force
Copy-Item docker-self-signed.pem $env:USERPROFILE\.aspnet\https -Force
Copy-Item docker-self-signed.key $env:USERPROFILE\.aspnet\https -Force
Copy-Item docker-self-signed.pfx $env:USERPROFILE\.aspnet\https -Force

# Copy to src folder to register as a root CA in client containers
Copy-Item docker-self-signed.pem ..\..\src\certificates\docker-self-signed.crt  -Force
