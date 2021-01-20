# Deploying Redis Cache

The ARM template `redisdeploy.json` and its parameter file (`redisdeploy.parameters.json`) are used to deploy following resources:

1. One Redis Cache

## Editing sbusdeploy.parameters.json file

You can edit the `redisdeploy.parameters.parameters.json` file to set your values, but is not needed. The only parameter than can
be set is:

1. `namespaceprefix` is a string that is used to create the Redis namespace. ARM script creates unique values by appending a unique string to this parameter value, so you can leave the default value.

## Deploy the template

Once parameter file is edited you can deploy it using [create-resources script](../readme.md).

i. e. if you are in windows, to deploy a Redis cache in a new Azure Resource Group located in westus, go to `deploy\az` folder and type:

```
create-resources.cmd redis\redisdeploy newResourceGroup -c westus
```









