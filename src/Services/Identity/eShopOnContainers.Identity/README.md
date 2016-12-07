# Containerized eShop - Identity Service
Sample reference containerized application, cross-platform and microservices architecture.
Powered by Microsoft

#Overview
This sample runs a microservices oriented application and a .net core Mvc application that consumes this services. You can find more information about how to set up docker in your machine in the global directory solution.

#Considerations
This service is a identity provider implemented with identity server 4, to implement this scenario is needed a public authority so to complete this flow in our local docker infrastructure we have to add a entry in our host file (windows) or equivalent in other platforms:

identity.service		127.0.0.1

In the root directory of this solution you'll find add-host.ps1 script that does this work for you (requires elevation permission).

#Deploy
In the global directory you will find the scripts needed to run and deploy the demo into your local docker infraestructure.

- <a href='build-image-services-identity.ps1'>build-image-services-identity.ps1</a>  <b>Build .net applications and docker images</b>: This power shell script that you will find in the <u>root directory of the solution</u> is the responsible of building .net applications and package in a pub folder and use docker commands to build the images needed to run the previously packaged .net applications.

- <b>Compose containers in your docker local VM</b>: Finally you have to open your favourite command tool pointing to the <u>root directory of this project</u> where docker-compose.yml file is located and run the command  `docker-compose up`

#Run
Once the deploy process of docker-compose finishes you have to be able to access the services in this urls:
- Identity service: http://localhost:5105
- Identity data (SQL Server): Server=localhost,5433;Database=aspnet-Microsoft.eShopOnContainers;User Id=sa;Password=Pass@word
