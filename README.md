# Containerized eShop
Sample reference containerized application, cross-platform and microservices architecture. 
Powered by Microsoft

<img src="img/eshop_cover.png">

## Overview
In this repo you can find a sample reference application that will help you to understand how to implement a microservice architecture based application using <b>.NET Core</b> and <b>Docker</b>.

The demo scenario is based on an eShop or eCommerce which is composed by a multi-container application. Each container is a microservice (basket-microservice, catalog-microservice, ordering-microservice and the  identity-microservice) which are developed using ASP.NET Core running on .NET Core so they can run either on Linux Containers and Windows Containers. Finally, those microservices are consumed by multiple client web and mobile apps, which are the following.

MVC Application: Its an MVC 6 development where you can find good samples about how to work with microservices in a MVC asp.net core application. 

SPA Application: Developed with Angular.js 2, Typescript and MVC 6. This is another approach for client web applications when you want to have a more modern behavior which is not having the typical browser round-trip on every action but behaving like a Single-Page-Application, more similar to a desktop app behavior.

Xamarin Application (Ios, Windows, Android): Its a client application that run in mobile devices (ios, android, windows) and you can find another example on how to build a microservices oriented application. 

## Development Environment Setup

#Tools
### Installing and configuring Docker in your development machine
<a href='https://github.com/docker/toolbox/releases/download/v1.12.3/DockerToolbox-1.12.3.exe'>Docker tools for windows</a>

####Mac
<a href='https://github.com/docker/toolbox/releases/download/v1.12.3/DockerToolbox-1.12.3.pkg'>Docker tools for Mac</a>

##Set up assinged Memory and CPU to Docker
In this application we run 3 instances of SQL Server running as containers plus 6 ASP.NET Core apps/services and 1 Redis server all of them running as Docker containers, as well. So it's important to set Docker up properly with enough memory RAM and CPU assigned to it. Once Docker for Windows is installed in your machine, enter into "Docker for Windows" Settings and its Advanced menu option so you are able to adjust the default values to the new values (Memory: Around 7GB and CPU:4) as shown in the image. Usually you might need a 16GB or 12GB memory machine for this configuration. If you have a less powerful machine, you can try with a lower configuration and/or by not starting certain containers like the basket and Redis. But if you don't start any of the containers, the application will not fully function properly, of course. 

<img src="img/docker_settings.png">

Bower and Gulp global installation
Before generating the Docker images, and specifically when generating the web apps binaries with "dotnet publish" from the custom stripts (like when running the build-images.ps1 script from PowerShell or the build-images.sh from bash in a Mac), you need to have access to the paths where you have installed Bower and Gulp. For that, the recomendation is to install Bower and Gulp with a global installation by running the following commands from command-line or bash:

**npm install -g bower** 

**npm install -g gulp**

Below you can see how those commands are run in Windows:
<img src="img/Bower_and_Gulp_setup.png">


#Deploy goblal
In the global directory you will find the scripts needed to run and deploy the demo into your local docker infraestructure. The steps: 


- <a href='build-images.ps1'>build-images.ps1</a>  <b>Build the .NET applications and Docker images</b>: This Power-Shell script that you will find in the <u>root directory of the solution</u> is the responsible for building the .NET applications, copy binaries and package in a pub folder and use Docker commands to build the images needed to run the containers. You can see how to run that PowerShell script in the screenshot below:
<img src="img/Generating_Docker_Images.png">

Once it finishes, you can check it out with Docker CLI if the images were generated correctly by typing in the PowerShell console:

**docker images**

You might have additional images, but at least, you should see the following list of images which are 6 custom images starting with the prefix "eshop" which is the name of the image repo:

<img src="img/list-of-images.png">

- <b>Compose containers in your docker local VM</b>: Finally you have to open your favourite command tool <u>pointing to the root directory of the solution</u> where docker-compose.yml file is located and run the command "docker-compose up"

when running "docker-compose up" you should see something similar to the following screenshot in the PowerShell command line, although it will be very long, even showing internal SQL commands from the services when populating the first time the sample data.

**docker-compose up**

<img src="img/docker-compose-up-1.png">

Note that the first time it detects that it needs the SQL Server image and the Redis image, it will pull or download the base images from the Internet, from the public repo at the Docker registry named DOCKER HUB by pulling the "microsoft/mssql-server-linux" which is the base image for the SQL Server for Linux on a Docker image, and the "library/redis" which is the base Redis image, so it might take more time until it spins up your custom containers.
The next time you run docker-compose up, since it'll have those base images already downloaded, it will just start the containers, like in the following screenshot:

<img src="img/docker-compose-up-2.png">

Once docker-compose up finishes after a few minutes, you will have that PowerShell showing output from the execution, so in order to ask to Docker about how it went, you need to open a new PowerShell command and type "docker ps" so you'll see all the running containers, as shown in the following screenshot.

<img src="img/docker-ps-with-all-microservices.png"> 
You can see the 6 custom containers running the microservices plus the 2 web applications. In adition you have the containers with the databases and the Redis cache for the basket data.

#Run
Once the deploy process of docker-compose finishes you have to be able to access the services in this urls from your machine:
- Web: http://localhost:5100
- Web Spa: http://localhost:5104
- Catalog service: http://localhost:5101
- Orders service: http://localhost:5102
- Basket service: http://localhost:5103
- Identity service: http://localhost:5105
- Orders data (SQL Server): Server=tcp:localhost,5432;Database=Microsoft.eShopOnContainers.Services.OrderingDb;User Id=sa;Password=Pass@word;
- Catalog data (SQL Server): Server=tcp:localhost,5434;Database=CatalogDB;User Id=sa;Password=Pass@word
- Identity data (SQL Server): Server=localhost,5433;Database=aspnet-Microsoft.eShopOnContainers;User Id=sa;Password=Pass@word
- Basket data (Redis): listening in localhost:6379

#Deploy individiual services into docker
Under each project root you will find a readme.md file as this that describes how to run and deploy the service individually into a docker container.



