kubectl apply -f ingress.yaml

# Deploy nginx-ingress core files
kubectl apply -f nginx-ingress\namespace.yaml
kubectl apply -f nginx-ingress\default-backend.yaml
kubectl apply -f nginx-ingress\configmap.yaml 
kubectl apply -f nginx-ingress\tcp-services-configmap.yaml
kubectl apply -f nginx-ingress\udp-services-configmap.yaml
kubectl apply -f nginx-ingress\without-rbac.yaml



