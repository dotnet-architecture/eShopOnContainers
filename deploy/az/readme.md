# Deploying resources using create-resources script

The `create-resources` script is a basic script to allow easy deployment of one ARM template in one resource group. You can deploy to an existing resource group or to create one.

## Deploying to a existing resource group

Just type `create-resources path-to-arm-template resourcegroup`. Called this way the script will:

1. Search for `path-to-arm-template.json` and `path-to-arm-template.parameters.json` files
2. If they exist, will deploy them in the `resourcegroup` specified (that has to exist).

## Deploying to a new resource group

Just type `create-resources path-to-arm-template resourcegroup -c location`. Called this way the script will:

1. Search for `path-to-arm-template.json` and `path-to-arm-template.parameters.json` files
2. If they exist, will create the `resourcegroup` specified in the `location` specified.
3. Finally will deploy `path-to-arm-template.json` and `path-to-arm-template.parameters.json` files in the `resourcegroup`


