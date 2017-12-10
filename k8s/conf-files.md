# YAML files used to deploy to k8s

This is just a brief enumeration of the configuration files used to create the k8s objects. Use as reference to find where specific object is.

- `deployments.yaml` Contains the definition of all deployments of the eShopOnContainers. Do not contain any infrastructure deployment (so no SQL, Redis, ...).
- `services.yaml` Contains the definition of all services of the eShopOnContainers. Do not contain any infrastructure service (so no SQL, Redis, ...).
- `basket-data.yaml` Contains the definition of the Redis (used by basket.api) deployment and service
- `nosql-data.yaml` Contains the definition of the Mongodb (used by locations and marketing) deployment and service
- `sql-data.yaml` Contains the definition of the SQL server deployment and service
- `rabbitmq.yaml` Contains the definition of the RabbitMQ deployment and service
- `keystore-data.yaml` Contains the deployment and service definition of the Redis used to mantain coherence between all the ASP.NET Identity keystores. 
- `conf_local.yaml` Contains the configuration map that configures all the Pods to use "local" containers (that is all containers in k8s)
- `conf_cloud.yaml` Contains the configuration map that configures all the Pods to use "cloud" resources (that is use Azure resources instead infrastructure containers). This file is provided with no valid values, just for example.
- `frontend.yaml` Contains the deployment and service definition of the NGINX frontend used as reverse-proxy

- For more information what kubernetes deployments are, read [Kubernetes help](https://kubernetes.io/docs/concepts/workloads/controllers/deployment/)
- For more information what kubernetes services are, read [Kubernetes help](https://kubernetes.io/docs/concepts/services-networking/service/)
