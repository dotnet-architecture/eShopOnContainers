Param(
    [parameter(Mandatory=$false)][string]$registry,
    [parameter(Mandatory=$false)][string]$dockerUser,
    [parameter(Mandatory=$false)][string]$dockerPassword,
    [parameter(Mandatory=$false)][bool]$deployCI,
    [parameter(Mandatory=$false)][bool]$useDockerHub,
    [parameter(Mandatory=$false)][string]$execPath,
    [parameter(Mandatory=$false)][string]$kubeconfigPath
)

function ExecKube($cmd) {    
    if($deployCI) {
        $kubeconfig = $kubeconfigPath + 'config';
        $exp = $execPath + 'kubectl ' + $cmd + ' --kubeconfig=' + $kubeconfig
        Invoke-Expression $exp
    }
    else{
        $exp = $execPath + 'kubectl ' + $cmd
        Invoke-Expression $exp
    }
}

# Not used when deploying through CI VSTS
if(-not $deployCI) {
        $requiredCommands = ("docker", "docker-compose", "kubectl")
        foreach ($command in $requiredCommands) {
        if ((Get-Command $command -ErrorAction SilentlyContinue) -eq $null) {
            Write-Host "$command must be on path" -ForegroundColor Red
            exit
        }
    }
}

# Use ACR instead of DockerHub as image repository
if(-not $useDockerHub) {
    Write-Host "Logging in to $registry" -ForegroundColor Yellow
    docker login -u $dockerUser -p $dockerPassword $registry
    if (-not $LastExitCode -eq 0) {
        Write-Host "Login failed" -ForegroundColor Red
        exit
    }

    # create registry key secret
    ExecKube -cmd 'create secret docker-registry registry-key `
    --docker-server=$registry `
    --docker-username=$dockerUser `
    --docker-password=$dockerPassword `
    --docker-email=not@used.com'
}

# Removing previous services & deployments
Write-Host "Removing existing services & deployments.." -ForegroundColor Yellow
ExecKube -cmd 'delete deployments --all'
ExecKube -cmd 'delete services --all'
ExecKube -cmd 'delete configmap config-files'
ExecKube -cmd 'delete configmap urls'

# start sql, rabbitmq, frontend deploymentsExecKube -cmd 'delete configmap config-files'
ExecKube -cmd 'create configmap config-files --from-file=nginx-conf=nginx.conf'
ExecKube -cmd 'label configmap config-files app=eshop'
ExecKube -cmd 'create -f sql-data.yaml -f basket-data.yaml -f keystore-data.yaml -f rabbitmq.yaml -f services.yaml -f frontend.yaml'

# building and publishing docker images not necessary when deploying through CI VSTS
if(-not $deployCI) {
    Write-Host "Building and publishing eShopOnContainers..." -ForegroundColor Yellow
    dotnet restore ../eShopOnContainers-ServicesAndWebApps.sln
    dotnet publish -c Release -o obj/Docker/publish ../eShopOnContainers-ServicesAndWebApps.sln

    Write-Host "Building Docker images..." -ForegroundColor Yellow
    docker-compose -p .. -f ../docker-compose.yml build    

    Write-Host "Pushing images to $registry..." -ForegroundColor Yellow
    $services = ("basket.api", "catalog.api", "identity.api", "ordering.api", "webmvc", "webspa", "webstatus")
    foreach ($service in $services) {
        docker tag eshop/$service $registry/eshop/$service
        docker push $registry/eshop/$service
    }
}

Write-Host "Waiting for frontend's external ip..." -ForegroundColor Yellow
while ($true) {
    $frontendUrl = & ExecKube -cmd 'get svc frontend -o=jsonpath="{.status.loadBalancer.ingress[0].ip}"'
    if ([bool]($frontendUrl -as [ipaddress])) {
        break
    }
    Start-Sleep -s 15
}

ExecKube -cmd 'create configmap urls `
    --from-literal=BasketUrl=http://basket `
    --from-literal=BasketHealthCheckUrl=http://basket/hc `
    --from-literal=CatalogUrl=http://$($frontendUrl)/catalog-api `
    --from-literal=CatalogHealthCheckUrl=http://catalog/hc `
    --from-literal=IdentityUrl=http://$($frontendUrl)/identity `
    --from-literal=IdentityHealthCheckUrl=http://identity/hc `
    --from-literal=OrderingUrl=http://ordering `
    --from-literal=OrderingHealthCheckUrl=http://ordering/hc `
    --from-literal=MvcClientExternalUrl=http://$($frontendUrl)/webmvc `
    --from-literal=WebMvcHealthCheckUrl=http://webmvc/hc `
    --from-literal=MvcClientOrderingUrl=http://ordering `
    --from-literal=MvcClientCatalogUrl=http://catalog `
    --from-literal=MvcClientBasketUrl=http://basket `
    --from-literal=WebSpaHealthCheckUrl=http://webspa/hc `
    --from-literal=SpaClientOrderingExternalUrl=http://$($frontendUrl)/ordering-api `
    --from-literal=SpaClientCatalogExternalUrl=http://$($frontendUrl)/catalog-api `
    --from-literal=SpaClientBasketExternalUrl=http://$($frontendUrl)/basket-api `
    --from-literal=SpaClientIdentityExternalUrl=http://$($frontendUrl)/identity `
    --from-literal=SpaClientExternalUrl=http://$($frontendUrl)'
    
ExecKube -cmd 'label configmap urls app=eshop'

Write-Host "Creating deployments..." -ForegroundColor Yellow

ExecKube -cmd 'create -f deployments.yaml'

# not using ACR for pulling images when deploying through CI VSTS
if(-not $deployCI) {
    # update deployments with the private registry before k8s tries to pull images
    # (deployment templating, or Helm, would obviate this)
    Write-Host "Update Image containers..." -ForegroundColor Yellow
    ExecKube -cmd 'set image deployments/basket basket=$registry/eshop/basket.api'
    ExecKube -cmd 'set image deployments/catalog catalog=$registry/eshop/catalog.api'
    ExecKube -cmd 'set image deployments/identity identity=$registry/eshop/identity.api'
    ExecKube -cmd 'set image deployments/ordering ordering=$registry/eshop/ordering.api'
    ExecKube -cmd 'set image deployments/webmvc webmvc=$registry/eshop/webmvc'
    ExecKube -cmd 'set image deployments/webstatus webstatus=$registry/eshop/webstatus'
    ExecKube -cmd 'set image deployments/webspa webspa=$registry/eshop/webspa'
}

Write-Host "Execute rollout..." -ForegroundColor Yellow
ExecKube -cmd 'rollout resume deployments/basket'
ExecKube -cmd 'rollout resume deployments/catalog'
ExecKube -cmd 'rollout resume deployments/identity'
ExecKube -cmd 'rollout resume deployments/ordering'
ExecKube -cmd 'rollout resume deployments/webmvc'
ExecKube -cmd 'rollout resume deployments/webstatus'
ExecKube -cmd 'rollout resume deployments/webspa'

Write-Host "WebSPA is exposed at http://$frontendUrl, WebMVC at http://$frontendUrl/webmvc, WebStatus at http://$frontendUrl/webstatus" -ForegroundColor Yellow

