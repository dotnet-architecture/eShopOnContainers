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

# Check command paths only for manual deployment
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

# start sql, rabbitmq, frontend deployments
ExecKube -cmd 'create configmap config-files --from-file=nginx-conf=nginx.conf'
ExecKube -cmd 'label configmap config-files app=eshop'
ExecKube -cmd 'create -f sql-data.yaml -f rabbitmq.yaml -f services.yaml -f frontend.yaml'

# building and publishing docker images not necessary when deploying through CI VSTS
if(-not $deployCI) {
    Write-Host "Building and publishing eShopOnContainers..." -ForegroundColor Yellow
    dotnet restore ../eShopOnContainers-ServicesAndWebApps.sln
    dotnet publish -c Release -o obj/Docker/publish ../eShopOnContainers-ServicesAndWebApps.sln

    Write-Host "Building Docker images..." -ForegroundColor Yellow
    docker-compose -p .. -f ../docker-compose.yml build

    Write-Host "Pushing images to $registry..." -ForegroundColor Yellow
    $services = ("basket.api", "catalog.api", "identity.api", "ordering.api", "webmvc", "webspa")
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
    --from-literal=BasketUrl=http://$($frontendUrl)/basket-api `
    --from-literal=CatalogUrl=http://$($frontendUrl)/catalog-api `
    --from-literal=IdentityUrl=http://$($frontendUrl)/identity `
    --from-literal=OrderingUrl=http://$($frontendUrl)/ordering-api `
    --from-literal=MvcClient=http://$($frontendUrl)/webmvc `
    --from-literal=SpaClient=http://$($frontendUrl)'

ExecKube -cmd 'label configmap urls app=eshop'

Write-Host "Creating deployments..."
ExecKube -cmd 'apply -f deployments.yaml'

# use ACR registry for pulling images if docker hub is not specified
if(-not $useDockerHub) {
    # update deployments with the private registry before k8s tries to pull images
    # (deployment templating, or Helm, would obviate this)
    ExecKube -cmd 'set image -f deployments.yaml `
        basket=$registry/eshop/basket.api `
        catalog=$registry/eshop/catalog.api `
        identity=$registry/eshop/identity.api `
        ordering=$registry/eshop/ordering.api `
        webmvc=$registry/eshop/webmvc `
        webspa=$registry/eshop/webspa'
}

ExecKube -cmd 'rollout resume -f deployments.yaml'

Write-Host "WebSPA is exposed at http://$frontendUrl, WebMVC at http://$frontendUrl/webmvc" -ForegroundColor Yellow
