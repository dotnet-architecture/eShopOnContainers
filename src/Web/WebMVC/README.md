# Containerized eShop - Web Mvc
Sample reference containerized application, cross-platform and microservices architecture.
Powered by Microsoft

#Overview
This sample runs a microservices oriented application and a .net core Mvc application that consumes this services. You can find more information about how to set up docker in your machine in the global directory solution.

#Deploy
In the global directory you will find the scripts needed to run and deploy the demo into your local docker infraestructure.

- <a href='build-image-web.ps1'>build-image-web.ps1</a>  <b>Build .net applications and docker images</b>: This power shell script that you will find in the <u>root directory of the solution</u> is the responsible of building .net applications and package in a pub folder and use docker commands to build the images needed to run the previously packaged .net applications.

- <b>Compose containers in your docker local VM</b>: Finally you have to open your favourite command tool pointing to the <u>root directory of this project</u> where docker-compose.yml file is located and run the command  `docker-compose up`

#Run
Once the deploy process of docker-compose finishes you have to be able to access the services in this urls:
- Web: http://localhost:5100
- Catalog service: http://localhost:5101
- Orders service: http://localhost:5102
- Basket service: http://localhost:5103
- Identity service: http://localhost:5105
- Orders data (SQL Server): Server=tcp:localhost,5432;Database=Microsoft.eShopOnContainers.Services.OrderingDb;User Id=sa;Password=Pass@word;
- Catalog data (SQL Server): Server=tcp:localhost,5434;Database=CatalogDB;User Id=sa;Password=Pass@word
- Identity data (SQL Server): Server=localhost,5433;Database=aspnet-Microsoft.eShopOnContainers;User Id=sa;Password=Pass@word
- Basket data (Redis): listening in localhost:6379






