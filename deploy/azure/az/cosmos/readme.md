# Deploying Azure Cosmosdb

The ARM template `deploycosmos.json` and its parameter file (`deploycosmos.parameters.json`) are used to deploy following resources:

1. One Cosmosdb database

## Editing deploycosmos.parameters.json file

You can edit the `deploycosmos.parameters.json` file to set your values, but is not needed. The only parameter that can
be set is:

1. `name` is a string that is used to create the database name. ARM script creates unique values by appending a unique string to this parameter value, so you can leave the default value.

## Deploy the template

Once parameter file is edited you can deploy it using [create-resources script](../readme.md).

i. e. if you are in windows, to deploy a Cosmosdb database in a new resourcegroup located in westus, go to `deploy\az` folder and type:

```
create-resources.cmd cosmos\deploycosmos newResourceGroup -c westus
```









