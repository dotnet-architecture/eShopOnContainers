kubectl apply -f ingress.yaml

# Deploy nginx-ingress core files
kubectl apply -f nginx-ingress$([IO.Path]::DirectorySeparatorChar)namespace.yaml
kubectl apply -f nginx-ingress$([IO.Path]::DirectorySeparatorChar)default-backend.yaml
kubectl apply -f nginx-ingress$([IO.Path]::DirectorySeparatorChar)configmap.yaml 
kubectl apply -f nginx-ingress$([IO.Path]::DirectorySeparatorChar)tcp-services-configmap.yaml
kubectl apply -f nginx-ingress$([IO.Path]::DirectorySeparatorChar)udp-services-configmap.yaml
kubectl apply -f nginx-ingress$([IO.Path]::DirectorySeparatorChar)without-rbac.yaml



