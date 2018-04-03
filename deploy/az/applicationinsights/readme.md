# Deploying Azure Application Insights

The ARM template `applicationinsights.json` and its parameter file (`applicationinsights.parameters.json`) are used to deploy Azure Application Insights

## Deploy the template

Once parameter file is edited you can deploy it using [create-resources script](../readme.md).

i. e. if you are in Windows, to deploy the Azure Azure Application Insights in a new resourcegroup located in westus, go to `deploy\az` folder and type:

```
create-resources.cmd applicationinsights\applicationinsights newResourceGroup -c westus
```

## Setting Azure Application Insights InstrumentationKey in for projects

Cope from Azure portal InstrumentationKey and add it in containers configuration.
For Kubernates - it can be done in k8s\conf_cloud.yml file as a value for Instrumentation_Key key.
For local docker environment remove # from INSTRUMENTATION_KEY key in .env file.