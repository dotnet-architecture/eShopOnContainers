REM az group create --name eShopOnAzureDevWin --location westus
az group deployment create --resource-group eShopOnAzureDevWin --parameters @mvparams.json --template-file azuredeploy.json

