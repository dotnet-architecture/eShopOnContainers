#!/bin/bash

CreateGroup()
{
	echo Creating resource group $resource_group in '$location'
	az group create --name $resource_group --location $location
}   # end of CreateGroup()



deployresources()
{
	echo Deploying ARM template '$path_and_filename.json' in resource group $resource_group
	az group deployment create --resource-group $resource_group --parameters @$path_and_filename.parameters.json --template-file $path_and_filename.json
}   # end of deployresources

Error_Usage()
{
	echo ""
	echo Usage: 
	echo create-resources arm-file resource-group-name [-c location]
	echo arm-file: Path to ARM template WITHOUT .json extension. An parameter file with same name plus '.parameters' MUST exist in same folder
	echo resource-grop-name: Name of the resource group to use or create
	echo -c: If appears means that resource group must be created. If -c is specified, must use enter location
	echo ""
	echo Examples:
	echo "1 create-resources path_and_filename testgroup (Deploys path_and_filename.json with parameters specified in path_and_filename.parameters.json file)."
	echo "2 create-resources path_and_filename newgroup -c westus (Deploys path_and_filename.json (with parameters specified in path_and_filename.parameters.json file) in a NEW resource group named newgroup in the westus location)"
}



if [ $# -le 1 ]; then
	Error_Usage
	exit 1
fi
if [ "$1" == "" ]; then
    echo "path_and_filename is empty"
	Error_Usage
	exit 1
fi

if [ "$2" == "" ]; then
    echo "Resource Group is empty"
	Error_Usage
	exit 1
fi

if [ ! -f "$1.json" ]; then
	echo "$1.json doesn't exist"
	exit 1
fi

if [ ! -f "$2.parameters.json" ]; then
	echo "$2.parameters.json doesn't exist"
	exit 1
fi


path_and_filename=$1
resource_group=$2


if [ "$3" == "-c" ]; then
    echo "Resource Group needs to be created"
	if [ "$4" == "" ]; then
		echo "but Resource Group name is missing"
		Error_Usage
		exit 1
	else
		location=$4
		CreateGroup
	fi
	
fi
deployresources

echo "all resources finished successfully"
