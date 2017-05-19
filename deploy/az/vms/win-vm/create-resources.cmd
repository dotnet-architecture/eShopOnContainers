if %1.==. GOTO error
if %2.!=-c. GOTO createvm
if %3.==. GOTO error
az group create --name %1 --location %3
createvm:
az group deployment create --resource-group %1 --parameters @mvparams.json --template-file azuredeploy.json
GOTO end
error:
@echo Usage: create-resources <resource-group-name> [-c location]
@echo <resource-grou-name>: Name of the resource group to use or create
@echo -c: If appears means that resource group must be created. If -c is specified, must use enter location
@echo Examples:
@echo create-resources testgroup (Creates VM in a existing testgroup resource group)
@echo create-resources newgroup -c westus (Creates the VM in a NEW resource group named newgroup in the westus location)
end:
