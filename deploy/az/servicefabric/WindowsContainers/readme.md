# Deploying Azure Service Fabric

The ARM template `servicefabricdeploy.json` and its parameter file (`servicefabricdeploy.parameters.json`) are used to create a service fabric cluster environment for windows containers.

## Editing servicefabricdeploy.parameters.json file

Edit the following params in `servicefabricdeploy.parameters.json` file to set your values:

- clusterName: Name of your SF cluster
- dnsName: Name assigned to your SF dns
- adminUserName: user name for administration
- adminPassword: user password for administration

Optionally, you can modify which ports are opened in the LoadBalancer for accessing externally to the apps:

- webMvcHttpPort: port externally exposed for the WebMVC app
- webSpaHttpPort: port externally exposed for the WebSPA app
- webStatusHttpPort: port externally exposed for the WebStatus app
- IdSrvHttpRule: port externally exposed for the Identity app

## Deploy the template

Once parameter file is edited you can deploy it using [create-resources script](../readme.md).

i. e. if you are in windows, to deploy sql databases in a new resourcegroup located in westus, go to `deploy\az` folder and type:

```
create-resources.cmd servicefabric\WindowsContainers\servicefabricdeploy newResourceGroup -c westus
```
## Deploy eShopOnServiceFabric with Visual Studio.

Alternatively, instead of using ARM templates, you can deploy eShop on service fabric directly by publishing the project eShopOnServiceFabric in eShopOnContainers-ServicesAndWebApps.sln with Visual Studio publish tool.




