set NODE_VERSION=8.11.1
curl -SL "https://nodejs.org/dist/v%NODE_VERSION%/node-v%NODE_VERSION%-win-x64.zip" --output nodejs.zip
tar -xf nodejs.zip  -C c:\
setx PATH "%PATH%;c:\node-v%NODE_VERSION%-win-x64"