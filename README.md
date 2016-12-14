# eShopOnContainers - Microservices Architecture and Containers based Reference Application
Sample .NET Core reference application, powered by Microsoft, based on a simplified microservices architecture and Docker containers. It is cross-platform thanks to .NET Core services capable of running on Linux or Windows containers depending on your Docker host. 

<img src="img/eshop_logo.png">
<img src="img/eShopOnContainers_Architecture_Diagram.png">
- Simplified Architecture Diagram of eShopOnContainers -

> ### @icon-info-circle Note on tested Docker Containers/Images
> The development and testing of this project was done on Docker Linux containers running in development machines with "Docker for Windows" and the default Hyper-V Linux VM (MobiLinuxVM) installed by "Docker for Windows". 
The Windows Containers scenario has not been tested, but the application should be able to run on Windows Containers, as well, as the .NET Core services have also been tested running on plain Windows (with no Docker).
The app was also partially tested on "Docker for Mac" using a development MacOS machine with .NET Core and VS Code installed. However, that is still a scenario using Linux containers running on the VM setup in the Mac by the "Docker for Windows" setup.
 

## Overview
In this repo you can find a sample reference application that will help you to understand how to implement a microservice architecture based application using <b>.NET Core</b> and <b>Docker</b>.

The example business domain or scenario is based on an eShop or eCommerce which is implemented as a multi-container application. Each container is a microservice deployment (like the basket-microservice, catalog-microservice, ordering-microservice and the  identity-microservice) which are developed using ASP.NET Core running on .NET Core so they can run either on Linux Containers and Windows Containers.
The screenshot below shows the VS Solution structure for those microservices/containers and client apps.

<img src="img/vs-solution-structure.png">

Finally, those microservices are consumed by multiple client web and mobile apps, as described below.

*MVC Application (ASP.NET Core)*: Its an MVC 6 development where you can find interesting scenarios on how to consume HTTP-based microservices from C# running in the server side, as it is a typical ASP.NET Core MVC application. 

*SPA (Single Page Application)*: Developed with Angular.js 2, Typescript and ASP.NET Core MVC 6. This is another approach for client web applications to be used when you want to have a more modern behavior which is not having the typical browser round-trip on every action but behaving like a Single-Page-Application, more similar to a desktop app behavior. The consumption of the HTTP-based microservices is done from TypeScript/JavaScript, in this case.

*Xamarin Mobile App (For iOS, Android and Windows/UWP)*: It is a client mobile app supporting the most common OS platforms (iOS, Android and Windows/UWP). In this case, the consumption of the microservices is done from C# but running on the client devices, so out of the Docker Host. 

## Development Environment Setup
### Requirements for Dec. 2016 version of eShopOnContainers

WINDOWS DEV MACHINE
- Visual Studio 2015 with latest Update
- .NET Core 1.0 (Including ASP.NET Core and VS Tooling)
- Bower and Gulp as global installs (See steps below)
- <a href='https://docs.docker.com/docker-for-windows/'>Docker for Windows</a>

MAC DEV MACHINE
- Visual Studio Code
- .NET Core 1.0 for Mac
- Bower and Gulp as global installs (See steps below)
- <a href='https://docs.docker.com/docker-for-mac/'>Docker for Mac</a>

### Installing and configuring Docker in your development machine

#### Set needed assigned Memory and CPU to Docker
In this application (Mid-December 2016 version) we run 3 instances of SQL Server running as containers plus 6 ASP.NET Core apps/services and 1 Redis server all of them running as Docker containers. So it's important to set Docker up properly with enough memory RAM and CPU assigned to it or you will get difficult errors when starting the containers with "docker-compose up". 
Once Docker for Windows/Mac is installed in your machine, enter into its Settings and the Advanced menu option so you are able to adjust it to the new values (Memory: Around 7GB and CPU:4) as shown in the image. Usually you might need a 16GB or 12GB memory machine for this configuration. If you have a less powerful machine, you can try with a lower configuration and/or by not starting certain containers like the basket and Redis. But if you don't start all the containers, the application will not fully function properly, of course. 

<img src="img/docker_settings.png">

#### Bower and Gulp global installation
Before generating the Docker images, and specifically when generating the web apps binaries with "dotnet publish" from the custom scripts (like when running the build-images.ps1 script from PowerShell or the build-images.sh from bash in a Mac), it needs to have access to the paths where you have installed Bower and Gulp. For that, the recommendation is to install Bower and Gulp with a global installation by running the following commands from command-line or bash:

`npm install -g bower` 

`npm install -g gulp`

Below you can see how those commands are run in Windows:
<img src="img/Bower_and_Gulp_setup.png">


## Build, Ship, Run 
In the global directory you will find the scripts needed to build, deploy and run the application into your local Docker development machine. The steps are the following: 

### Compile the .NET apps and Build the Docker images
- Open a PowerShell window in Windows, move to the root folder of your solution and run the <b>build-images.ps1</b> script file like in the following screenshot.
- This Power-Shell script that you will find in the <u>root directory of the solution</u> is responsible for building the .NET applications, copy the binaries in a pub folder and use Docker CLI commands to build the custom Docker images needed to run the containers. You can see how to run that PowerShell script in the screenshot below:
<img src="img/Generating_Docker_Images.png">

- Once it finishes, you can check it out with Docker CLI if the images were generated correctly by typing in the PowerShell console the command: `docker images`
<img src="img/list-of-images.png">
Those Docker images are the ones you have available in your local image repository in your machine.
You might have additional images, but at least, you should see the following list of images which are 6 custom images starting with the prefix "eshop" which is the name of the image repo. The rest of the images that are not starting with "eshop" will probably be official base-images like the microsoft/aspnetcore or the SQL Server for Linux images.

### Deploy containers into your Docker host
You can deploy Docker containers to a regularDocker host either by using the `docker run` command which need to be executed once per microservice, or by using the CLI tool `docker-compose up` which is very convenient for multi-container applications as it can spin-up all the multiple containers in your application with a single command. These are the steps:
- <b>Run your containers in your local host</b>: Open your favorite command tool (PowerShell od CommandLine in Windows / Bash in Mac) <u> and move to the root directory of the solution</u> where the docker-compose.yml file is located and run the command `docker-compose up`. When running "docker-compose up" you should see something similar to the following screenshot in the PowerShell command window, although it will much longer than that, also showing many internal SQL commands from the services when populating the first time the sample data.
`docker-compose up`
<img src="img/docker-compose-up-1.png">

- Note that the first time you run any container (with docker run or docker-compose) it detects that it needs the base images we are using, like the SQL Server image and the Redis image, so it will pull or download those base images from the Internet, from the public repo at the Docker registry named DOCKER HUB, by pulling the "microsoft/mssql-server-linux" which is the base image for the SQL Server for Linux on containers, and the "library/redis" which is the base Redis image. Therefore, the first time you run docker-compose it might take a few minutes pulling those images before it spins up your custom containers.
The next time you run docker-compose up, since it'll have those base images already pulled/downloaded, it will just start the containers, like in the following screenshot:
<img src="img/docker-compose-up-2.png">

- <b>Check out the containers running in your Docker host</b>:Once docker-compose up finishes after a few minutes, you will have that PowerShell showing the execution's output in a "wait state", so in order to ask to Docker about "how it went" and see what containers are running, you need to open a second PowerShell window and type "docker ps" so you'll see all the running containers, as shown in the following screenshot.
<img src="img/docker-ps-with-all-microservices.png"> 
You can see the 6 custom containers running the microservices plus the 2 web applications. In adition you have the containers with the SQL databases and the Redis cache for the basket microservice data.

### Test the application and the microservices
#### IMPORTANT: Modify your local "hosts" file by setting an IP related to the identity.service network name
- Due to the fact that when trying to authenticate against the STS (Security Token Service) container the browser is redirected to it from outside the docker host (your browser in your PC), it needs to have an IP reachable from outside the Docker Host. Therefore you need to add an entry like `10.0.75.1 identity.service` or `127.0.0.1 identity.service` to your <b>hosts</b> file in your local dev machine.
- If you don't want to hassle with it, just run the PowerShell script named `add-host-entry.ps1` from the solution root folder.
- If the STS were running in an external IP in a server, in the Internet or in any cloud like Azure, since that IP is reachable from anywhere, you wouldn't need to configure your hosts file. However, that IP would need to be updated into your docker-compose.yml file, in the identity.service URLs. 
#### Test the applications and microservices 
Once the deploy process of docker-compose finishes you should be able to access the services in the following URLs or connection string, from your dev machine:
- Web MVC: http://localhost:5100
- Web Spa: http://localhost:5104
- Catalog microservice: http://localhost:5101
- Ordering microservice: http://localhost:5102
- Basket microservice: http://localhost:5103
- Identity microservice: http://localhost:5105
- Orders database (SQL Server): Server=tcp:localhost,5432;Database=Microsoft.eShopOnContainers.Services.OrderingDb;User Id=sa;Password=Pass@word;
- Catalog database (SQL Server): Server=tcp:localhost,5434;Database=CatalogDB;User Id=sa;Password=Pass@word
- ASP.NET Identity database (SQL Server): Server=localhost,5433;Database=aspnet-Microsoft.eShopOnContainers;User Id=sa;Password=Pass@word
- Basket data (Redis): listening at localhost:6379

### Deploying individiual services into docker
Under each project root you will find a readme.md file which describes how to run and deploy the service individually into a docker host.

> ### @icon-info-circle Note on Windows Containers
> As mentioned, the development and testing of this project was done on Docker Linux containers running in development machines with "Docker for Windows" and the default Hyper-V Linux VM (MobiLinuxVM) installed by "Docker for Windows". 
In order to run the application on Windows Containers you'd need to change the base images used by each container:
> - Official .NET Core base-image for Windows Containers, at Docker Hub: https://hub.docker.com/r/microsoft/dotnet/ (Using the Windows Nanoserver tag)
> - Official base-image for SQL Server on Windows Containers, at Docker Hub: https://hub.docker.com/r/microsoft/mssql-server-windows


