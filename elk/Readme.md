This article contains a brief introduction to centralized structured logging with [Serilog](https://serilog.net/) and event viewing with [ELK](https://www.elastic.co/elk-stack) in eShopOnContainers. ELK is an acronym of ElasticSearch, LogStash and Kibana. This is one of the most used tools in the industry standards.

![](img/elk/kibana-working.png)

## Wiring eshopOnContainers with ELK in Localhost

eshopOnContainers is ready for work with ELK, you only need to setup the configuration parameter **LogstashUrl**, in **Serilog** Section, for achieve this, you can do it modifing this parameter in every appsettings.json of every service, or via Environment Variable **Serilog:LogstashUrl**.

There is another option, a zero-configuration environment for testing the integration launching via ```docker-compose``` command, on the root directory of eshopOnContainers:

```sh
docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.elk.yml build

docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.elk.yml up
```

### Configuring Logstash index on Kibana

Once time you have started and configured your application, you only need to configure the logstash index on kibana.
You can address to Kibana, with docker-compose setup is at [http://localhost:5601](http://localhost:5601)

If you have accessed to kibana too early, you can see this error. It's normal, depending of your machine the kibana stack needs a bit of time to startup.
![](img/elk/kibana_startup.png)

You can wait a bit and refresh the page, the first time you enter, you need to configure and index pattern, in the ```docker-compose``` configuration, the index pattern name is **eshops-\***.
![](img/elk/kibana_eshops_index.png)

With the index pattern configured, you can enter in the discover section and start viewing how the tool is recollecting the logging information.

![](img/elk/kibana_result.png)

## Configuring ELK on Azure VM
Another option is to use a preconfigured virtual machine with Logstash, ElasticSearch and Kibana and point the configuration parameter **LogstashUrl**. For doing this you can address to Microsoft Azure, and start searching a Certified ELK Virtual Machine

![](img/elk/create-vm-elk-azure.png)

This options it have a certified preconfigured options (Network, VirtualMachine type, OS, RAM, Disks) for having a good starting point of ELK with good performance.

![](img/elk/create-vm-elk-azure-summary.png)

When you have configured the main aspects of your virtual machine, you will have a review&create last step like this:
![](img/elk/create-vm-elk-azure-last-step.png)