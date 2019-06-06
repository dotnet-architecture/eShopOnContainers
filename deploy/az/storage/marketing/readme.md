# Deploying Marketing Storage 

The ARM template `deploystorage.json` and its parameter file (`deploystorage.parameters.json`) are used to deploy following resources:

1. One Storage Account
2. One CDN profile
3. One Endpoint

## Editing deploystorage.parameters.json file

You can edit the `deploystorage.parameters.json` file to set your values, but is not needed. The only parameters that can
be set are:

1. `marketingstorage` is a string that is used to create the storage account name. ARM script creates unique values by appending a unique string to this parameter value, so you can leave the default value.

2. `profileName` is a string that is used to create the CDN profile name.

3. `endpointName` is a string that is used to create the storage endpoint name. ARM script creates unique values by appending a unique string to this parameter value, so you can leave the default value.

## Deploy the template

Once parameter file is edited you can deploy it using [create-resources script](../../readme.md).

i. e. if you are in windows, to deploy a Storage account in a new resourcegroup located in westus, go to `deploy\az` folder and type:

```
create-resources.cmd storage\marketing\deploystorage newResourceGroup -c westus
```









