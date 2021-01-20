# Deploying SQL Server & SQL Databases

The ARM template `sqldeploy.json` and its parameter file (`sqldeploy.parameters.json`) are used to deploy following resources:

1. One SQL Server
2. Three SQL databases (for ordering, catalog and identity) services.
3. Firewall rules to **allow access from any IP to SQL Server**. This allows easy management, but is not desired in production environments.

## Editing sqldeploy.parameters.json file

You **must** edit the `sqldeploy.parameters.json` file to set login and password of the admin user.

1. `sql_server` is a object parameter that contains the sql server name and the database names. You can leave default values if you want.
2. `admin` is a string with the admin logon. You MUST provide a valid value
3. `adminpwd` is a string with the admin password. You MUST provide a valid value

ARM script ensures uniqueness of the SQL server created by appending one unique string in its name (defined in the `sql_server.name` parameter).

## Deploy the template

Once parameter file is edited you can deploy it using [create-resources script](../readme.md).
i. e. if you are in windows, to deploy sql databases in a new resourcegroup located in westus, go to `deploy\az` folder and type:

```
create-resources.cmd sql\sqldeploy newResourceGroup -c westus
```










