# Specify the Istio version that will be leveraged throughout these instructions
$ISTIO_VERSION="1.0.6"

# Windows

$ProgressPreference = 'SilentlyContinue'; 
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Invoke-WebRequest -URI "https://github.com/istio/istio/releases/download/$ISTIO_VERSION/istio-$ISTIO_VERSION-win.zip" -OutFile "istio-$ISTIO_VERSION.zip"
Expand-Archive -Path "istio-$ISTIO_VERSION.zip" -DestinationPath .

cd istio-$ISTIO_VERSION
New-Item -ItemType Directory -Force -Path "C:\Program Files\Istio"
mv ./bin/istioctl.exe "C:\Program Files/Istio/"
$PATH = [environment]::GetEnvironmentVariable("PATH", "User")
[environment]::SetEnvironmentVariable("PATH", $PATH + "; C:\Program Files\Istio", "User")

