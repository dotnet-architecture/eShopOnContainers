
# =======================
kubectl annotate --overwrite ingress eshop-webmvc  nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 
kubectl annotate --overwrite ingress eshop-webmvc nginx.ingress.kubernetes.io/proxy-body-size=8M
kubectl annotate --overwrite ingress eshop-webmvc nginx.ingress.kubernetes.io/client-body-buffer-size=1M

kubectl annotate --overwrite ingress eshop-webspa nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 
kubectl annotate --overwrite ingress eshop-webspa nginx.ingress.kubernetes.io/proxy-body-size=8M
kubectl annotate --overwrite ingress eshop-webspa nginx.ingress.kubernetes.io/client-body-buffer-size=1M

kubectl annotate --overwrite ingress eshop-webstatus  nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 
kubectl annotate --overwrite ingress eshop-webstatus nginx.ingress.kubernetes.io/proxy-body-size=8M
kubectl annotate --overwrite ingress eshop-webstatus nginx.ingress.kubernetes.io/client-body-buffer-size=1M

kubectl annotate --overwrite ingress eshop-apigwms nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 
kubectl annotate --overwrite ingress eshop-apigwms nginx.ingress.kubernetes.io/proxy-body-size=8M
kubectl annotate --overwrite ingress eshop-apigwms nginx.ingress.kubernetes.io/client-body-buffer-size=1M

kubectl annotate --overwrite ingress eshop-apigwws  nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 
kubectl annotate --overwrite ingress eshop-apigwws nginx.ingress.kubernetes.io/proxy-body-size=8M
kubectl annotate --overwrite ingress eshop-apigwws nginx.ingress.kubernetes.io/client-body-buffer-size=1M

kubectl annotate --overwrite ingress eshop-identity-api nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 
kubectl annotate --overwrite ingress eshop-identity-api nginx.ingress.kubernetes.io/proxy-body-size=8M
kubectl annotate --overwrite ingress eshop-identity-api nginx.ingress.kubernetes.io/client-body-buffer-size=1M

kubectl annotate --overwrite ingress eshop-webhooks-web nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 
kubectl annotate  --overwrite ingress eshop-webhooks-web nginx.ingress.kubernetes.io/proxy-body-size=8M
kubectl annotate --overwrite ingress eshop-webhooks-web nginx.ingress.kubernetes.io/client-body-buffer-size=1M

kubectl annotate --overwrite ingress eshop-webhooks-api nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 
kubectl annotate --overwrite ingress eshop-webhooks-api nginx.ingress.kubernetes.io/proxy-body-size=8M
kubectl annotate --overwrite ingress eshop-webhooks-api nginx.ingress.kubernetes.io/client-body-buffer-size=1M

kubectl annotate --overwrite ingress eshop-webhooks-web nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 
kubectl annotate --overwrite ingress eshop-webhooks-web nginx.ingress.kubernetes.io/proxy-body-size=8M
kubectl annotate --overwrite ingress eshop-webhooks-web nginx.ingress.kubernetes.io/client-body-buffer-size=1M

# -------------
kubectl annotate --overwrite ingress eshop-webmvc  nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 
kubectl annotate --overwrite ingress eshop-webspa  nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 
kubectl annotate --overwrite ingress eshop-webstatus  nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 
kubectl annotate --overwrite ingress eshop-apigwms  nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 
kubectl annotate --overwrite ingress eshop-apigwws  nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 
kubectl annotate --overwrite ingress eshop-identity-api  nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 
kubectl annotate --overwrite ingress eshop-webhooks-api  nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 
kubectl annotate --overwrite ingress eshop-webhooks-web nginx.ingress.kubernetes.io/proxy-buffer-size="16k" 

# nginx.ingress.kubernetes.io/proxy-buffer-size: "128k"
# nginx.ingress.kubernetes.io/proxy-buffers-number: "4"
# nginx.ingress.kubernetes.io/proxy-body-size: 8M
# nginx.ingress.kubernetes.io/client-body-buffer-size: 1M

# nginx.ingress.kubernetes.io/server-snippet: |
#   http2_max_header_size 256k;
#   http2_max_field_size 256k;
