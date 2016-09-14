<#
.SYNOPSIS
Deploys an ASP .NET Core Web Application into a docker container running in a specified Docker machine.

.DESCRIPTION
The following script will execute a set of Docker commands against the designated dockermachine.

.PARAMETER Build
Builds the containers using docker-compose build.

.PARAMETER Clean
Clears out any running containers (docker-compose kill, docker-compose rm -f).

.PARAMETER Exec
Executes a command in the container using docker exec.

.PARAMETER GetUrl
Gets the url for the site to open.

.PARAMETER WaitForUrl
Waits for url to respond.

.PARAMETER Refresh
Kills the running command in the container, publishes the project and restarts executing the command.

.PARAMETER Run
Removes any conflicting containers running on the same port, then instances the containers using docker-compose up.

.PARAMETER Environment
Specifies the configuration under which the project will be built and run (Debug or Release).

.PARAMETER Machine
Specifies the docker machine name to connect to. This is optional and if left blank or not provided it will use the currently configured docker host, or if no host is set, will use the Docker for Windows beta.

.PARAMETER ProjectFolder
Specifies the project folder, defaults to the parent of the directory containing this script.

.PARAMETER ProjectName
Specifies the project name used by docker-compose, defaults to the name of $ProjectFolder.

.PARAMETER NoCache
Specifies the build argument --no-cache.

.PARAMETER OpenSite
Specifies whether to launch the site once the docker container is running, defaults to $True.

.PARAMETER RemoteDebugging
Specifies if remote debugging is needed, defaults to $False.

.PARAMETER ClrDebugVersion
Specifies the version of the debugger, defaults to 'VS2015U2'.

.PARAMETER Command
Specifies the command to run in the container.

.INPUTS
None. You cannot pipe inputs to DockerTask.

.EXAMPLE
Compiles the project and builds the docker image using the currently configured docker host, or when no host is set, used for the Docker for Windows beta. To see the container running, use the -Run parameter
C:\PS> .\DockerTask.ps1 -Build -Environment Release

.EXAMPLE
Will use 'docker-compose up' on the project, using the docker-machine instance named 'default', and opens a browser once the container responds. Assumes -Build was previously run. For the Docker for Windows Beta, remove the -Machine parameter or pass '' as the value.
C:\PS> .\DockerTask.ps1 -Run -Environment Release -Machine 'default'

.LINK
http://aka.ms/DockerToolsForVS
#>

Param(
    [Parameter(ParameterSetName = "Build", Position = 0, Mandatory = $True)]
    [switch]$Build,
    [Parameter(ParameterSetName = "Clean", Position = 0, Mandatory = $True)]
    [switch]$Clean,
    [Parameter(ParameterSetName = "Run", Position = 0, Mandatory = $True)]
    [switch]$Run,
    [Parameter(ParameterSetName = "Exec", Position = 0, Mandatory = $True)]
    [switch]$Exec,
    [Parameter(ParameterSetName = "GetUrl", Position = 0, Mandatory = $True)]
    [switch]$GetUrl,
    [Parameter(ParameterSetName = "WaitForUrl", Position = 0, Mandatory = $True)]
    [switch]$WaitForUrl,
    [Parameter(ParameterSetName = "Refresh", Position = 0, Mandatory = $True)]
    [switch]$Refresh,
    [Parameter(ParameterSetName = "ValidateVolumeMapping", Position = 0, Mandatory = $True)]
    [switch]$ValidateVolumeMapping,
    [parameter(ParameterSetName = "Clean", Position = 1, Mandatory = $True)]
    [parameter(ParameterSetName = "Build", Position = 1, Mandatory = $True)]
    [parameter(ParameterSetName = "Run", Position = 1, Mandatory = $True)]
    [parameter(ParameterSetName = "Refresh", Position = 1, Mandatory = $True)]
    [ValidateNotNullOrEmpty()]
    [String]$Environment,
    [parameter(ParameterSetName = "Clean", Position = 2, Mandatory = $False)]
    [parameter(ParameterSetName = "Build", Position = 2, Mandatory = $False)]
    [parameter(ParameterSetName = "Run", Position = 2, Mandatory = $False)]
    [parameter(ParameterSetName = "Exec", Position = 1, Mandatory = $False)]
    [Parameter(ParameterSetName = "GetUrl", Position = 1, Mandatory = $False)]
    [Parameter(ParameterSetName = "WaitForUrl", Position = 1, Mandatory = $False)]
    [parameter(ParameterSetName = "Refresh", Position = 2, Mandatory = $False)]
    [parameter(ParameterSetName = "ValidateVolumeMapping", Position = 1, Mandatory = $False)]
    [String]$Machine,
    [parameter(ParameterSetName = "Clean", Position = 3, Mandatory = $False)]
    [parameter(ParameterSetName = "Build", Position = 3, Mandatory = $False)]
    [parameter(ParameterSetName = "Run", Position = 3, Mandatory = $False)]
    [parameter(ParameterSetName = "Exec", Position = 2, Mandatory = $False)]
    [parameter(ParameterSetName = "Refresh", Position = 3, Mandatory = $False)]
    [ValidateNotNullOrEmpty()]
    [String]$ProjectFolder = (Split-Path -Path $MyInvocation.MyCommand.Definition),
    [parameter(ParameterSetName = "Clean", Position = 4, Mandatory = $False)]
    [parameter(ParameterSetName = "Build", Position = 4, Mandatory = $False)]
    [parameter(ParameterSetName = "Run", Position = 4, Mandatory = $False)]
    [parameter(ParameterSetName = "Exec", Position = 3, Mandatory = $False)]
    [parameter(ParameterSetName = "Refresh", Position = 4, Mandatory = $False)]
    [ValidateNotNullOrEmpty()]
    [String]$ProjectName = (Split-Path -Path (Resolve-Path $ProjectFolder) -Leaf).ToLowerInvariant(),
    [parameter(ParameterSetName = "Build", Position = 5, Mandatory = $False)]
    [switch]$NoCache,
    [parameter(ParameterSetName = "Run", Position = 5, Mandatory = $False)]
    [bool]$OpenSite = $True,
    [parameter(ParameterSetName = "Run", Position = 6, Mandatory = $False)]
    [bool]$RemoteDebugging = $False,
    [parameter(ParameterSetName = "Build", Position = 6, Mandatory = $False)]
    [String]$ClrDebugVersion = "VS2015U2",
    [parameter(ParameterSetName = "Exec", Position = 4, Mandatory = $True)]
    [parameter(ParameterSetName = "Refresh", Position = 5, Mandatory = $True)]
    [ValidateNotNullOrEmpty()]
    [String]$Command
)

$ErrorActionPreference = "Stop"

# Turns VERBOSE output ON
$VerbosePreference = "Continue"

# Docker Working Directory for validating volume mapping. Should be in sync with Dockerfile and Docker.props.
$DockerWorkingDirectory = "/app/"

# Path for the launch URL to be opened
$launchURLPath = "api/environmentInfo/machinename"

# The project name can only contain alphanumeric charecters, replace everything else with empty string
$ProjectName = $ProjectName -replace "[^a-zA-Z0-9]", ""

# The name of the image created by the compose file
$ImageName = "username/microsoft.eshoponcontainers.services.ordering.api"

# Calculate the name of the container created by the compose file
$ContainerName = "${ProjectName}_microsoft.eshoponcontainers.services.ordering.api"

# .net core runtime ID for the container (used to publish the app correctly)
$RuntimeID = "debian.8-x64"
# .net core framework (used to publish the app correctly)
$Framework = "netcoreapp1.0"

# Kills all containers using an image, removes all containers using an image, and removes the image.
function Clean () {
    $composeFilePath = GetComposeFilePath($ProjectFolder)

    # Call compose-down to clean up the containers
    $shellCommand = "docker-compose -f '$composeFilePath' -p $ProjectName down"
    Write-Verbose "Executing: $shellCommand"
    Invoke-Expression "cmd /c $shellCommand `"2>&1`""
    if ($LastExitCode -ne 0) {
        Write-Error "Failed to clean up the containers"
    }

    # If $ImageName exists remove it
    $ImageNameRegEx = "\b$ImageName\b"
    docker images | select-string -pattern $ImageNameRegEx | foreach {
        $imageName = $_.Line.split(" ", [System.StringSplitOptions]::RemoveEmptyEntries)[0];
        $tag = $_.Line.split(" ", [System.StringSplitOptions]::RemoveEmptyEntries)[1];
        $shellCommand = "docker rmi -f ${imageName}:$tag"
        Write-Verbose "Executing: $shellCommand";
        Invoke-Expression "cmd /c $shellCommand `"2>&1`""
    }

    # Remove any dangling images (from previous builds)
    $shellCommand = "docker images -q --filter 'dangling=true'"
    Write-Verbose "Executing: $shellCommand"
    $danglingImages = $(Invoke-Expression "cmd /c $shellCommand `"2>&1`"")
    if (-not [String]::IsNullOrWhiteSpace($danglingImages)) {
        $shellCommand = "docker rmi -f $danglingImages"
        Write-Verbose "Executing: $shellCommand"
        Invoke-Expression "cmd /c $shellCommand `"2>&1`""
    }

    # If the folder for publishing exists, delete it
    if (Test-Path $pubPath) {
        Remove-Item $pubPath -Force -Recurse
    }
}

# Runs docker build.
function Build () {
    # Publish the project
    PublishProject

    $composeFilePath = GetComposeFilePath($pubPath)

    $buildArgs = ""
    if ($NoCache)
    {
        $buildArgs = "--no-cache"
    }

    # Call docker-compose on the published project to build the images
    $shellCommand = "docker-compose -f '$composeFilePath' -p $ProjectName build $buildArgs"
    Write-Verbose "Executing: $shellCommand"
    Invoke-Expression "cmd /c $shellCommand `"2>&1`""
    if ($LastExitCode -ne 0) {
        Write-Error "Failed to build the image"
    }
}

function GetContainerId () {
    $containerId = (docker ps -f "name=$ContainerName" -q -n=1)
    if ([System.String]::IsNullOrWhiteSpace($containerId)) {
        Write-Error "Could not find a container named $ContainerName"
    }

    $containerId
}

# Validates volume mapping
function ValidateVolumeMapping () {
    # Volume mapping enables shared folder mounting between host and docker container
    # If there are no files in the working directory, most likely volume mapping is misconfigured.
    $containerId = GetContainerId
    Write-Host "Validating volume mapping in the container $containerId"
    $shellCommand = "docker exec -i $containerId /bin/bash -c 'ls $DockerWorkingDirectory'"
    if (!$(Invoke-Expression $shellCommand)) {
        Write-Error "Unable to validate volume mapping. For troubleshooting, follow instructions from http://aka.ms/DockerToolsTroubleshooting"
    }
}

# Runs docker run
function Run () {
    $composeFilePath = GetComposeFilePath($pubPath)

    $conflictingContainerIds = $(docker ps | select-string -pattern ":80->" | foreach { Write-Output $_.Line.split()[0] })

    if ($conflictingContainerIds) {
        $conflictingContainerIds = $conflictingContainerIds -Join ' '
        Write-Host "Stopping conflicting containers using port 80"
        $stopCommand = "docker stop $conflictingContainerIds"
        Write-Verbose "Executing: $stopCommand"
        Invoke-Expression "cmd /c $stopCommand `"2>&1`""
        if ($LastExitCode -ne 0) {
            Write-Error "Failed to stop the container(s)"
        }
    }

    $shellCommand = "docker-compose -f '$composeFilePath' -p $ProjectName up -d"
    Write-Verbose "Executing: $shellCommand"
    Invoke-Expression "cmd /c $shellCommand `"2>&1`""
    if ($LastExitCode -ne 0) {
        Write-Error "Failed to start the container(s)"
    }

    if ($OpenSite) {
        OpenSite
    }
}

# Runs docker run
function Exec () {
    $containerId = GetContainerId
    $shellCommand = "docker exec -i $containerId $Command"
    Write-Verbose "Executing: $shellCommand"
    Invoke-Expression $shellCommand
    if ($LastExitCode -ne 0) {
        Write-Error "Failed to exec command $Command in the container"
    }
}

# Opens the remote site
function OpenSite () {
    # If we're going to debug, the server won't start immediately; don't need to wait for it.
    if (-not $RemoteDebugging)
    {
        $uri = GetUrl

        WaitForUrl $uri

        # Open the site.
        Start-Process $uri
    }
    else {
        # Give the container 10 seconds to get ready
        Start-Sleep 10
    }
} 

# Gets the Url of the remote container
function GetUrl () {
    if ([System.String]::IsNullOrWhiteSpace($Machine)) {
        $launchURL = [System.UriBuilder]"http://localhost"
    }
    else {
        $launchURL = [System.UriBuilder]"http://$(docker-machine ip $Machine)"
    }
    $launchURL.Path = $launchURLPath
    return $launchURL.Uri.AbsoluteUri
}

# Checks if the URL is responding
function WaitForUrl ([string]$uri) {
    Write-Host "Opening site $uri " -NoNewline
    $status = 0
    $count = 0

    #Check if the site is available
    while ($status -ne 200 -and $count -lt 120) {
        try {
            Write-Host "Trying to connect to $uri ($count/120)"
            $response = Invoke-WebRequest -Uri $uri -Headers @{"Cache-Control"="no-cache";"Pragma"="no-cache"} -UseBasicParsing -Verbose:$false
            $status = [int]$response.StatusCode
        }
        catch [System.Net.WebException] { }
        if($status -ne 200) {
            # Wait Time max. 2 minutes (120 sec.)
            Start-Sleep 1
            $count += 1
        }
    }
    Write-Host
    if($status -ne 200) {
        # Check if bad volume mapping is the reason why we were not able to connect
        ValidateVolumeMapping
    }
}

function Refresh () {
    # Find the container
    $containerId = GetContainerId

    # Kill any existing process
    $shellCommand = "docker exec -i $containerId /bin/bash -c 'if PID=`$(pidof -x $Command); then kill `$PID; fi'"
    Write-Verbose "Executing: $shellCommand"
    Invoke-Expression $shellCommand

    # Publish the project
    PublishProject

    # Restart the process
    $shellCommand = "docker exec -i $containerId $Command"
    Write-Verbose "Executing: $shellCommand"
    Invoke-Expression $shellCommand
    if ($LastExitCode -ne 0) {
        Write-Error "Failed to exec command $Command in the container"
    }
}

# Publishes the project
function PublishProject () {
    $oldPath = $Env:Path

    try {
        # Need to add $ProjectFolder\node_modules\.bin and the External Tools folder from the Web Tools to Path before calling publish
        $newPath = (Join-Path $ProjectFolder ".\node_modules\.bin") + ";$oldPath"

        # Find where VS is installed
        $vsPath = $null
        if (Test-Path HKLM:\Software\WOW6432Node\Microsoft\VisualStudio\14.0\) {
            $vsPath = (Get-ItemProperty -Path HKLM:\Software\WOW6432Node\Microsoft\VisualStudio\14.0\ -Name ShellFolder).ShellFolder
        } elseif (Test-Path HKLM:\Software\Microsoft\VisualStudio\14.0\) {
            $vsPath = (Get-ItemProperty -Path HKLM:\Software\Microsoft\VisualStudio\14.0\ -Name ShellFolder).ShellFolder
        }

        # Find where the Web Tools are installed
        if ($vsPath -ne $null) {
            $webExternalPath = $null
            # Check for the Web Tools in VS
            if (Test-Path (Join-Path $vsPath "Web")) {
                $webExternalPath = Join-Path $vsPath (Join-Path "Web" "External")
            # or the Web Exress edition
            } elseif (Test-Path (Join-Path $vsPath "WebExpress")) {
                $webExternalPath = Join-Path $vsPath (Join-Path "WebExpress" "External")
            }
            # If the Web Tools were found, add the externals from the Web Tools to Path
            if ($webExternalPath -ne $null) {
                $newPath = "$newPath;$webExternalPath;$webExternalPath\git"
            }
        }

        # Set Path to our new path
        $Env:Path = $newPath

        # Publish the project
        dotnet publish -f $Framework -r $RuntimeID -c $Environment -o $pubPath $ProjectFolder
        if ($? -eq $False) {
            Write-Error "Failed to publish the project"
        }
    }
    finally {
        # Restore path to its old value
        $Env:Path = $oldPath
    }
}

function GetComposeFilePath([string]$folder) {
    $composeFileName = "docker-compose.yml"
    if ($Environment -ne "Release") {
        $composeFileName = "docker-compose.$($Environment.ToLower()).yml"
    }
    $composeFilePath = Join-Path $folder $composeFileName

    if (Test-Path $composeFilePath) {
        return $composeFilePath
    } else {
        Write-Error -Message "$Environment is not a valid parameter. File '$composeFilePath' does not exist." -Category InvalidArgument
    }
}

# Need the full path of the project for mapping
$ProjectFolder = Resolve-Path $ProjectFolder

if (![System.String]::IsNullOrWhiteSpace($Machine)) {
    $users = Split-Path $env:USERPROFILE -Parent

    # Set the environment variables for the docker machine to connect to
    $shellCommand = "docker-machine env $Machine --shell powershell"
    Write-Verbose "Executing: $shellCommand | Invoke-Expression"
    Invoke-Expression $shellCommand | Invoke-Expression
    if ($LastExitCode -ne 0) {
        Write-Error "Failed to set docker environment variables"
    }

    # Get the driver name of the docker machine
    $DriverName = (docker-machine inspect $Machine | Out-String | ConvertFrom-Json)."DriverName"

    # If the driver is virtualbox, need to check that the project location can be volume mapped
    if ($DriverName -eq "virtualbox") {
        if (!$ProjectFolder.StartsWith($users, [StringComparison]::InvariantCultureIgnoreCase)) {
            $message  = "VirtualBox by default shares C:\Users as c/Users. If the project is not under c:\Users, please manually add it to the shared folders on VirtualBox. "`
                      + "Follow instructions from https://www.virtualbox.org/manual/ch04.html#sharedfolders"
            Write-Warning -Message $message
        }
        elseif (!$ProjectFolder.StartsWith($users, [StringComparison]::InvariantCulture)) {
            # If the project is under C:\Users, fix the casing if necessary. Path in Linux is case sensitive and the default shared folder c/Users
            # on VirtualBox can only be accessed if the project folder starts with the correct casing C:\Users as in $env:USERPROFILE
            $ProjectFolder = $users + $ProjectFolder.Substring($users.Length)
        }
    }
}

# Our working directory in bin
$dockerBinFolder = Join-Path $ProjectFolder (Join-Path "bin" "Docker")
# The folder to publish the app to
$pubPath = Join-Path (Join-Path $dockerBinFolder $Environment) "app"

Write-Verbose "Setting: `$env:CLRDBG_VERSION = `"$ClrDebugVersion`""
$env:CLRDBG_VERSION = "$ClrDebugVersion"

if ($RemoteDebugging) {
    Write-Verbose "Setting: `$env:REMOTE_DEBUGGING = 1"
    $env:REMOTE_DEBUGGING = 1
}
else {
    Write-Verbose "Setting: `$env:REMOTE_DEBUGGING = 0"
    $env:REMOTE_DEBUGGING = 0
}

# Call the correct functions for the parameters that were used
if ($Clean) {
    Clean
}
if ($Build) {
    Build
}
if ($Run) {
    Run
}
if ($Exec) {
    Exec
}
if ($GetUrl) {
    GetUrl
}
if ($WaitForUrl) {
    WaitForUrl (GetUrl)
}
if ($Refresh) {
    Refresh
}
if ($ValidateVolumeMapping) {
    ValidateVolumeMapping
}
