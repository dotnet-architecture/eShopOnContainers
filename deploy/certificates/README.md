# Setup dev certificates deploying to Docker Desktop

1. Create a self-signed certificate
2. Install certificates
3. Configure the services

## 1 - Create the self-signed certificate (`.pem + .key`) and its `.pfx` file

**From WSL**, run the `create-docker-certificate.sh` script with a strong password for the certificate.

```bash
./create-docker-certificate.sh "secure-COMPLEX-and-SECRET-password"
```

The script creates a certificate for both `host.docker.internal` and `localhost`.

### 2 - Install the certificates

Run the `install-docker-certificate.ps1` with the same password you used above:

```powershell
.\install-docker-certificate.ps1 "secure-COMPLEX-and-SECRET-password"
```

The above script:

1. Imports the certificate in the current user root CA store.
2. Copies the certificate files to the `%USERPROFILE%\.aspnet\https` folder. Servers will serve the certificate from this folder.
3. Copies the `.pem` file as `.crt` to the src\certificates folder to add it as a root CA when building the images for some services.

### 3 - Configure some services to serve the certificates

1. Copy the `src\docker-compose.certificates.sample.yml` file as `src\docker-compose.certificates.yml`
2. Configure the password you assigned to the certificates in the settings `ASPNETCORE_Kestrel__Certificates__Default__Password`

> **IMPORTANT**
>
> The `src\docker-compose.certificates.yaml` file is .gitignore'd to avoid pushing it to the repo with the certificate password.
>
> To avoid security risks, **DON'T FORCE PUSH the file**.
