REM az group create --name eShopOnAzureDev --location westus

az group deployment create --resource-group eShopOnAzureDev --parameters @mvparams.json ^
  --template-uri https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/docker-simple-on-ubuntu/azuredeploy.json

