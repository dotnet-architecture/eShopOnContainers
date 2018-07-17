# Deploying Azure Service Bus

The ARM template `sbusdeploy.json` and its parameter file (`sbusdeploy.parameters.json`) are used to deploy following resources:

1. One Service Bus namespace
2. One Service Bus topic
3. Subscriptions used by application

## Editing sbusdeploy.parameters.json file

You can edit the `sbusdeploy.parameters.parameters.json` file to set your values, but is not needed. The only parameter than can
be set is:

1. `namespaceprefix` is a string that is used to create the namespace. ARM script creates unique values by appending a unique string to this parameter value, so you can leave the default value.

## Deploy the template

Once parameter file is edited you can deploy it using [create-resources script](../readme.md).

i. e. if you are in windows, to deploy servicebus in a new resourcegroup located in westus, go to `deploy\az` folder and type:

```
create-resources.cmd servicebus\sbusdeploy newResourceGroup -c westus
```