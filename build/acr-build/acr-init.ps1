Param (
    # The name of the Azure Container Registry
    [Parameter(Mandatory = $true)]
    [string]
    $AcrName,

    # Specifies GitHub Username
    [Parameter(Mandatory = $true)]
    [string]
    $Username,

    # The name of the GitHub Repository
    [Parameter(Mandatory = $false)]
    [string]
    $RepositoryName = "eShopOnContainers",

    # The source control Branch Name
    [Parameter(Mandatory = $false)]
    [string]
    $BranchName = "dev",

    # The GitHub Personal Access Token
    [Parameter(Mandatory = $true)]
    [string]
    $GitAccessToken,

    # Indicates whether the base image trigger is enabled
    [Parameter(Mandatory = $false)]
    [bool]
    $BaseImageTrigger = $true,

    # Indicates whether the source control commit trigger is enabled
    [Parameter(Mandatory = $false)]
    [bool]
    $CommitTrigger = $true,

    # Trigger task after creating
    [Parameter(Mandatory = $false)]
    [bool]
    $TriggerNow = $true
)


$Context = "https://github.com/$Username/$RepositoryName.git"

$Services = @(
    @{ Name = "eshopbasket"; Image = "eshop/basket.api"; File = "src/Services/Basket/Basket.API/Dockerfile" },
    @{ Name = "eshopcatalog"; Image = "eshop/catalog.api"; File = "src/Services/Catalog/Catalog.API/Dockerfile" },
    @{ Name = "eshopidentity"; Image = "eshop/identity.api"; File = "src/Services/Identity/Identity.API/Dockerfile" },
    @{ Name = "eshopordering"; Image = "eshop/ordering.api"; File = "src/Services/Ordering/Ordering.API/Dockerfile" },
    @{ Name = "eshoporderingbg"; Image = "eshop/ordering.backgroundtasks"; File = "src/Services/Ordering/Ordering.BackgroundTasks/Dockerfile" },
    @{ Name = "eshopmarketing"; Image = "eshop/marketing.api"; File = "src/Services/Marketing/Marketing.API/Dockerfile" },
    @{ Name = "eshopwebspa"; Image = "eshop/webspa"; File = "src/Web/WebSPA/Dockerfile" },
    @{ Name = "eshopwebmvc"; Image = "eshop/webmvc"; File = "src/Web/WebMVC/Dockerfile" },
    @{ Name = "eshopwebstatus"; Image = "eshop/webstatus"; File = "src/Web/WebStatus/Dockerfile" },
    @{ Name = "eshoppayment"; Image = "eshop/payment.api"; File = "src/Services/Payment/Payment.API/Dockerfile" },
    @{ Name = "eshoplocations"; Image = "eshop/locations.api"; File = "src/Services/Location/Locations.API/Dockerfile" },
    @{ Name = "eshopocelotapigw"; Image = "eshop/ocelotapigw"; File = "src/ApiGateways/ApiGw-Base/Dockerfile" },
    @{ Name = "eshopmobileshoppingagg"; Image = "eshop/mobileshoppingagg"; File = "src/ApiGateways/Mobile.Bff.Shopping/aggregator/Dockerfile" },
    @{ Name = "eshopwebshoppingagg"; Image = "eshop/webshoppingagg"; File = "src/ApiGateways/Web.Bff.Shopping/aggregator/Dockerfile" },
    @{ Name = "eshoporderingsignalrhub"; Image = "eshop/ordering.signalrhub"; File = "src/Services/Ordering/Ordering.SignalrHub/Dockerfile" },
    @{ Name = "eshopwebhooks"; Image = "eshop/webhooks.api"; File = "src/Services/Webhooks/Webhooks.API/Dockerfile" },
    @{ Name = "eshopwebhooksclient"; Image = "eshop/webhooks.client"; File = "src/Web/WebhookClient/Dockerfile" }
)

$NumberOfServices = $Services.Count


Write-Host "Creating $NumberOfServices build tasks ..." -ForegroundColor Green

$k = 1

$Services | ForEach-Object {

    $bname = $_.Name
    $bimg = $_.Image
    $bfile = $_.File

    Write-Host "[$k/$NumberOfServices] Setting ACR Build for `"$bname`" ($bimg) ..." -ForegroundColor Green
    az acr task create --registry $AcrName --name $bname --image ${bimg}:$BranchName --context $Context --branch $BranchName --git-access-token $GitAccessToken --file $bfile `
        --base-image-trigger-enabled $BaseImageTrigger --commit-trigger-enabled $CommitTrigger

    if ($TriggerNow) {
        Write-Host "Triggering Build for `"$bname`" ..." -ForegroundColor Green
        az acr task run --registry $AcrName --name $bname
    }

    $k += 1
}
