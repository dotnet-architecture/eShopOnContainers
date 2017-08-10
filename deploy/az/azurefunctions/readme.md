# Deploying Azure Functions

The ARM template `azurefunctionsdeploy.json` and its parameter file (`azurefunctionsdeploy.parameters.json`) are used to deploy Marketing azure functions.

## Editing azurefunctionsdeploy.parameters.json file

You can edit the `azurefunctionsdeploy.parameters.parameters.json` file to set your values, but is not needed. 

## Deploy the template

Once parameter file is edited you can deploy it using [create-resources script](../readme.md).

i. e. if you are in windows, to deploy sql databases in a new resourcegroup located in westus, go to `deploy\az` folder and type:

```
create-resources.cmd azurefunctions\azurefunctionsdeploy newResourceGroup -c westus
```
## Deploy Marketing azure function with Visual Studio.

Alternatively, instead of using ARM templates, you can deploy Marketing azure function directly by publishing the project Marketing-functions in eShopOnContainers-AzureFunctions.sln with Visual Studio publish tool.

## Setting Azure function configurations

Once deployed, go to azure portal and set the connection string for the azure function under the name "SqlConnection". The value must be the connection string which points to MarketingDB.

Example:  

"SqlConnection": "Server=tcp:eshopsql.database.windows.net,1433;Initial Catalog=marketingdb;"

In appsettings section, add a new entry named "MarketingStorageUri". The value must be the uri of the blob storage where the campaign images are stored.

Example:

"MarketingStorageUri": "https://marketingcampaign.blob.core.windows.net/pics/"



