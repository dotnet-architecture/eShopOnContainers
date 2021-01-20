# Kubernetes 101
## Docker vs. Kubernetes
Docker helps you package applications into images, and execute them in containers. Kubernetes is a robust platform for containerized applications. It abstracts away the underlying network infrastructure and hardware required to run them, simplifying their deployment, scaling, and management.

## Kubernetes from the container up
### Pods
The basic unit of a Kubernetes deployment is the **Pod**. A Pod encapsulates one or more containers. For example, the `basket` Pod specifies two containers:
>`deployments.yaml`
>
>The first container runs the `eshop/basket.api` image:
>```yaml
>spec:
>  containers:
>  - name: basket
>    image: eshop/basket.api
>    env:
>    - name: ConnectionString
>      value: 127.0.0.1
>```
>Note the `ConnectionString` environment variable: containers within a Pod are networked via `localhost`. The second container runs the `redis` image:
>```yaml
>- name: basket-data
>  image: redis:3.2-alpine
>  ports:
>  - containerPort: 6379
>```
Placing `basket` and `basket-data` in the same Pod is reasonable here because the former requires the latter, and owns all its data. If we wanted to scale the service, however, it would be better to place the containers in separate Pods because the basket API and redis scale at different rates.

If the containers were in separate Pods, they would no longer be able to communicate via `localhost`; a **Service** would be required.

### Services
Services expose Pods to external networks. For example, the `basket` Service exposes Pods with labels `app=eshop` and `component=basket` to the cluster at large:
>`services.yaml`
>```yaml
>kind: Service
>metadata:
>  ...
>  name: basket
>spec:
>  ports:
>  - port: 80
>  selector:
>    app: eshop
>    component: basket
>```
Kubernetes's built-in DNS service resolves Service names to cluster-internal IP addresses. This allows the nginx frontend to proxy connections to the app's microservices by name:
>`nginx.conf`
>```
>location /basket-api {
>    proxy_pass http://basket;
>```
The frontend Pod is different in that it needs to be exposed outside the cluster. This is accomplished with another Service:
>`frontend.yaml`
>```yaml
>spec:
>  ports:
>  - port: 80
>    targetPort: 8080
>  selector:
>    app: eshop
>    component: frontend
>  type: LoadBalancer
>```
`type: LoadBalancer` tells Kubernetes to expose the Service behind a load balancer appropriate for the cluster's platform. For Azure Container Service, this creates an Azure load balancer rule with a public IP.

### Deployments
Kubernetes uses Pods to organize containers, and Services to network them. It uses **Deployments** to organize creating, and modifying, Pods. A Deployment describes a state of one or more Pods. When a Deployment is created or modified, Kubernetes attempts to realize that state.

The Deployments in this project are basic. Still, `deploy.ps1` shows some more advanced Deployment capabilities. For example, Deployments can be paused. Each Deployment of this app is paused at creation:
>`deployments.yaml`
>```yaml
>kind: Deployment
>spec:
>  paused: true
>```
This allows the deployment script to change images before Kubernetes creates the Pods:
>`deploy.ps1`
>```powershell
>kubectl set image -f deployments.yaml basket=$registry/basket.api ...
>kubectl rollout resume -f deployments.yaml
>```

### ConfigMaps
A **ConfigMap** is a collection of key/value pairs commonly used to provide configuration information to Pods. The deployment script uses one to store the frontend's configuration:
>`deploy.ps1`
>```
>kubectl create configmap config-files from-file=nginx-conf=nginx.conf
>```
This creates a ConfigMap named `config-files` with key `nginx-conf` whose value is the content of nginx.conf. The frontend Pod mounts that value as `/etc/nginx/nginx.conf`:
>`frontend.yaml`
>```yaml
>spec:
>  containers:
>  - name: nginx
>  ...
>  volumeMounts:
>  - name: config
>    mountPath: /etc/nginx
>  volumes:
>  - name: config
>    configMap:
>      name: config-files
>      items:
>      - key: nginx-conf
>        path: nginx.conf
>```
This facilitates rapid iteration better than other techniques, e.g. building an image to bake in configuration.

The script also stores public URLs for the app's components in a ConfigMap:
>`deploy.ps1`
>```powershell
>kubectl create configmap urls --from-literal=BasketUrl=http://$($frontendUrl)/basket-api ...
>```
>Here's how the `webspa` Deployment uses it:
>
>`deployments.yaml`
>```yaml
>spec:
>  containers:
>  - name: webspa
>    ...
>    env:
>      ...
>      - name: BasketUrl
>        valueFrom:
>          configMapKeyRef:
>            name: urls
>            key: BasketUrl
>```

### Further reading
* [Kubernetes Concepts](https://kubernetes.io/docs/concepts/)
* [kubectl for Docker Users](https://kubernetes.io/docs/user-guide/docker-cli-to-kubectl/)
* [Kubernetes API reference](https://kubernetes.io/docs/api-reference/v1.5/)