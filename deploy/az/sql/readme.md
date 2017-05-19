# Deploying SQL Server & SQL Databases

The ARM template `sqldeploy.json` and its parameter file (`sqldeploy.parameters.json`) are used to deploy following resources:

1. One SQL Server
2. Three SQL databases (for ordering, catalog and identity) services.
3. Firewall rules to **allow access from any IP to SQL Server**. This allows easy management, but is not desired in production environments.

## Editing sqldeploy.parameters.json file

You have to edit the `sqldeploy.parameters.json` file to set your values. There are two parameters:

1. `sql_server` is a object parameter that contains the sql server name, the admin login and password, and the database names.
2. `suffix` is a suffix that will be added to thee sql_server name to ensure uniqueness.

## Deploy the template

Once parameter file is edited you can deploy it using [create-resources script](../readme.md).

i. e. if you are in windows, to deploy sql databases in a new resourcegroup located in westus, go to `deploy\az` folder and type:

```
create-resources.cmd sql\sqldeploy newResourceGroup -c westus
```









