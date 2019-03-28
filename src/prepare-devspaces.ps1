# This script just copies app.yaml and inf.yaml files to all devspaces projects.
# This is to workaround issue #56 - https://github.com/Azure/dev-spaces/issues/56

Write-Host "Copying app.yaml and inf.yaml to Mobile.Bff.Marketing" -ForegroundColor Yellow
Copy-Item "..\k8s\helm\app.yaml" -Destination ".\ApiGateways\Mobile.Bff.Marketing\apigw" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\ApiGateways\Mobile.Bff.Marketing\apigw" -Force

Write-Host "Copying app.yaml and inf.yaml to Web.Bff.Marketing" -ForegroundColor Yellow
Copy-Item "..\k8s\helm\app.yaml" -Destination ".\ApiGateways\Web.Bff.Marketing\apigw" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\ApiGateways\Web.Bff.Marketing\apigw" -Force

Write-Host "Copying app.yaml and inf.yaml to Mobile.Bff.Shopping" -ForegroundColor Yellow
Copy-Item "..\k8s\helm\app.yaml" -Destination ".\ApiGateways\Mobile.Bff.Shopping\apigw" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\ApiGateways\Mobile.Bff.Shopping\apigw" -Force

Write-Host "Copying app.yaml and inf.yaml to Web.Bff.Shopping" -ForegroundColor Yellow
Copy-Item "..\k8s\helm\app.yaml" -Destination ".\ApiGateways\Web.Bff.Shopping\apigw" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\ApiGateways\Web.Bff.Shopping\apigw" -Force

Write-Host "Copying app.yaml and inf.yaml to Mobile.Bff Shopping Aggregator" -ForegroundColor Yellow
Copy-Item "..\k8s\helm\app.yaml" -Destination ".\ApiGateways\Mobile.Bff.Shopping\aggregator" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\ApiGateways\Mobile.Bff.Shopping\aggregator" -Force

Write-Host "Copying app.yaml and inf.yaml to Web.Bff Shopping Aggregator" -ForegroundColor Yellow
Copy-Item "..\k8s\helm\app.yaml" -Destination ".\ApiGateways\Web.Bff.Shopping\aggregator" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\ApiGateways\Web.Bff.Shopping\aggregator" -Force

Write-Host "Copying app.yaml and inf.yaml to Basket API" -ForegroundColor Yellow
Copy-Item "..\k8s\helm\app.yaml" -Destination ".\Services\Basket\Basket.API" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\Services\Basket\Basket.API" -Force

Write-Host "Copying app.yaml and inf.yaml to Catalog API" -ForegroundColor Yellow
Copy-Item "..\k8s\helm\app.yaml" -Destination ".\Services\Catalog\Catalog.API" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\Services\Catalog\Catalog.API" -Force

Write-Host "Copying app.yaml and inf.yaml to Identity API" -ForegroundColor Yellow
Copy-Item "..\k8s\helm\app.yaml" -Destination ".\Services\Identity\Identity.API" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\Services\Identity\Identity.API" -Force

Write-Host "Copying app.yaml and inf.yaml to Locations API" -ForegroundColor Yellow
Copy-Item "..\k8s\helm\app.yaml" -Destination ".\Services\Location\Locations.API" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\Services\Location\Locations.API" -Force

Write-Host "Copying app.yaml and inf.yaml to Marketing API" -ForegroundColor Yellow
Copy-Item "..\k8s\helm\app.yaml" -Destination ".\Services\Marketing\Marketing.API" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\Services\Marketing\Marketing.API" -Force

Write-Host "Copying app.yaml and inf.yaml to Ordering API" -ForegroundColor Yellow
Copy-Item "..\k8s\helm\app.yaml" -Destination ".\Services\Ordering\Ordering.API" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\Services\Ordering\Ordering.API" -Force

Copy-Item "..\k8s\helm\app.yaml" -Destination ".\Services\Ordering\Ordering.SignalrHub" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\Services\Ordering\Ordering.SignalrHub" -Force

Copy-Item "..\k8s\helm\app.yaml" -Destination ".\Services\Ordering\Ordering.BackgroundTasks" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\Services\Ordering\Ordering.BackgroundTasks" -Force

Write-Host "Copying app.yaml and inf.yaml to Payment API" -ForegroundColor Yellow
Copy-Item "..\k8s\helm\app.yaml" -Destination ".\Services\Payment\Payment.API" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\Services\Payment\Payment.API" -Force

Write-Host "Copying app.yaml and inf.yaml to Webhooks API" -ForegroundColor Yellow
Copy-Item "..\k8s\helm\app.yaml" -Destination ".\Services\Webhooks\Webhooks.API" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\Services\Webhooks\Webhooks.API" -Force

Write-Host "Copying app.yaml and inf.yaml to WebMVC" -ForegroundColor Yellow
Copy-Item "..\k8s\helm\app.yaml" -Destination ".\Web\WebMVC" -Force
Copy-Item "..\k8s\helm\inf.yaml" -Destination ".\Web\WebMVC" -Force




