# Deploying Azure Service Fabric

The ARM template `servicefabricdeploy.json` and its parameter file (`servicefabricdeploy.parameters.json`) are used to create a service fabric cluster environment.

## Editing servicefabricdeploy.parameters.json file

You can edit the `servicefabricdeploy.parameters.parameters.json` file to set your values, but is not needed. 

## Deploy the template

Once parameter file is edited you can deploy it using [create-resources script](../readme.md).

i. e. if you are in windows, to deploy sql databases in a new resourcegroup located in westus, go to `deploy\az` folder and type:

```
create-resources.cmd servicefabric\servicefabricdeploy newResourceGroup -c westus
```
## Deploy eShopOnServiceFabric with Visual Studio.

Alternatively, instead of using ARM templates, you can deploy eShop on service fabric directly by publishing the project eShopOnServiceFabric in eShopOnContainers-ServicesAndWebApps.sln with Visual Studio publish tool.




