# eShopOnContainers - Microservices Architecture and Containers based Reference Application (**BETA state** - Visual Studio 2017 and CLI environments compatible)
Sample .NET Core reference application, powered by Microsoft, based on a simplified microservices architecture and Docker containers. <p>

**NOTE ON VS 2017 VERSION NEEDED**

For the code at the MASTER branch (this one), you can use VS 2017 RTM v 15.6.

For the code at the DEV branch, you need use VS 2017 RTM v 15.7 (Currently in PREVIEW state).

For both branches, you can also use Docker CLI (docker-compose up)

**Note for Pull Requests (PRs)**: We accept pull request from the community. When doing it, please do it onto the **DEV branch** which is the consolidated work-in-progress branch. **Do not request it onto Master branch**, if possible.

**NEWS / ANNOUNCEMENTS**
Do you want to be up-to-date on .NET Architecture guidance and reference apps like eShopOnContainers? --> Subscribe by "WATCHING" this new GitHub repo: https://github.com/dotnet-architecture/News 

## Updated for .NET Core 2.0 "wave" of technologies
NOTE: We have migrated the whole server-side solution to .NET Core 2.0 "wave". Not just compilation but also new recommended code in EF Core 2.0, ASP.NET Core 2.0, and other new related versions.

The **dockerfiles** in the solution have also been updated and now support [**Docker Multi-Stage**](https://blogs.msdn.microsoft.com/stevelasker/2017/09/11/net-and-multistage-dockerfiles/) since mid-December 2017.

For a list on the new .NET Core 2.0 related implemented features, see this [blog post](https://blogs.msdn.microsoft.com/dotnet/2017/08/02/microservices-and-docker-containers-architecture-patterns-and-development-guidance/).

>**PLEASE** Read our [branch guide](./branch-guide.md) to know about our branching policy

> ### DISCLAIMER
> **IMPORTANT:** The current state of this sample application is **BETA**, because we are constantly evolving towards new released technologies. Therefore, many areas could be improved and change significantly while refactoring current code and implementing new features. Feedback with improvements and pull requests from the community will be highly appreciated and accepted.
>
> This reference application proposes a simplified microservice oriented architecture implementation to introduce technologies like .NET Core with Docker containers through a comprehensive application. The chosen domain is an eShop/eCommerce but simply because it is a well-know domain by most people/developers.
However, this sample application should not be considered as an "eCommerce reference model", at all. The implemented business domain might not be ideal from an eCommerce business point of view. It is neither trying to solve all the problems in a large, scalable and mission-critical distributed system. It is just a bootstrap for developers to easily get started in the world of Docker containers and microservices with .NET Core.
> <p>For example, the next step after running the solution in the local dev PC and understanding Docker containers and microservices development with .NET Core, is to select a microservice cluster/orchestrator like Kubernetes in Azure or Azure Service Fabric, both environments tested and supported by this solution.
> Additional steps would be to move your databases to HA cloud services (like Azure SQL Database), or to implement your EventBus with Azure Service Bus or any other production ready Service Bus in the market.
> <p> 
> <img src="img/exploring-to-production-ready.png">
> Read the planned <a href='https://github.com/dotnet/eShopOnContainers/wiki/01.-Roadmap-and-Milestones-for-future-releases'>Roadmap and Milestones for future releases of eShopOnContainers</a> within the Wiki for further info about possible new implementations and provide feedback at the  <a href='https://github.com/dotnet/eShopOnContainers/issues'>ISSUES section</a> if you'd like to see any specific scenario implemented or improved. Also, feel free to discuss on any current issue.

**Architecture overview**: This reference application is cross-platform at the server and client side, thanks to .NET Core services capable of running on Linux or Windows containers depending on your Docker host, and to Xamarin for mobile apps running on Android, iOS or Windows/UWP plus any browser for the client web apps.
The architecture proposes a simplified microservice oriented architecture implementation with multiple autonomous microservices (each one owning its own data/db) and implementing different approaches within each microservice (simple CRUD vs. DDD/CQRS patterns) using Http as the communication protocol between the client apps and the microservices and supports asynchronous communication for data updates propagation across multiple services based on Integration Events and an Event Bus (a light message broker, to choose between RabbitMQ or Azure Service Bus, underneath) plus other features defined at the <a href='https://github.com/dotnet/eShopOnContainers/wiki/01.-Roadmap-and-Milestones-for-future-releases'>roadmap</a>.
<p>
<img src="img/eshop_logo.png">
<img src="img/eShopOnContainers_Architecture_Diagram.png">
<p>

> ### Important Note on AP Gateways and published microservice APIs
> Note that the previous architecture is a how you deploy it in a local Docker development machine. For a production-ready architecture we recommend to keep evolving your architecture with additional features like API Gateways based on AzureAPI Management or any other approach for API Gateways explained in the related documentation/eBook.
<p>
<img src="img/eShopOnContainers-Architecture-With-Azure-API-Management.png">
<p>

> The sample code in this repo is NOT making use of Azure API Management in order to be able to provide an "F5 experience" in Visual Studio (or CLI) of the sample with no up-front dependencies in Azure. But you should evaluate API Gateways alternatives when building for production.

The microservices are different in type, meaning different internal architecture patterns approaches depending on it purpose, as shown in the image below.
<p>
<img src="img/eShopOnContainers-Architecture-With-Azure-API-Management.png">
<p>

> The sample code in this repo is NOT making use of Azure API Management in order to be able to provide an "F5 experience" in Visual Studio (or CLI) of the sample with no up-front dependencies in Azure. But you should evaluate API Gateways alternatives when building for production.

The microservices are different in type, meaning different internal architecture pattern approaches depending on its purpose, as shown in the image below.
<p>
<img src="img/eShopOnContainers_Types_Of_Microservices.png">
<p>
<p>

> ### Important Note on Database Servers/Containers
> In this solution's current configuration for a development environment, the SQL databases are automatically deployed with sample data into a single SQL Server for Linux container (a single shared Docker container for SQL databases) so the whole solution can be up and running without any dependency to any cloud or specific server. Each database could also be deployed as a single Docker container, but then you'd need more than 8GB of RAM assigned to Docker in your development machine in order to be able to run 3 SQL Server Docker containers in your Docker Linux host in "Docker for Windows" or "Docker for Mac" development environments.
> <p> A similar case is defined in regard to Redis cache running as a container for the development environment. Or a No-SQL database (MongoDB) running as a container.
> <p> However, in a real production environment it is recommended to have your databases (SQL Server, Redis, and the NO-SQL database, in this case) in HA (High Available) services like Azure SQL Database, Redis as a service and Azure CosmosDB instead the MongoDB container (as both systems share the same access protocol). If you want to change to a production configuration, you'll just need to change the connection strings once you have set up the servers in a HA cloud or on-premises.

## Related documentation and guidance
While developing this reference application, we've been creating a reference <b>Guide/eBook</b> focusing on <b>architecting and developing containerized and microservice based .NET Applications</b> (download link available below) which explains in detail how to develop this kind of architectural style (microservices, Docker containers, Domain-Driven Design for certain microservices) plus other simpler architectural styles, like monolithic apps that can also live as Docker containers.
<p>
There are also additional eBooks focusing on Containers/Docker lifecycle (DevOps, CI/CD, etc.) with Microsoft Tools, already published plus an additional eBook focusing on Enterprise Apps Patterns with Xamarin.Forms.
You can download them and start reviewing these Guides/eBooks here:
<p>

| Architecting & Developing | Containers Lifecycle & CI/CD | App patterns with Xamarin.Forms |
| ------------ | ------------|  ------------|
| <a href='https://aka.ms/microservicesebook'><img src="img/ebook_arch_dev_microservices_containers_cover.png"> </a> | <a href='https://aka.ms/dockerlifecycleebook'> <img src="img/ebook_containers_lifecycle.png"> </a> | <a href='https://aka.ms/xamarinpatternsebook'> <img src="img/xamarin-enterprise-patterns-ebook-cover-small.png"> </a> |
| <sup> <a href='https://aka.ms/microservicesebook'>**Download .PDF** (2nd Edition)</a> </sup>  | <sup> <a href='https://aka.ms/dockerlifecycleebook'>**Download** </a>  </sup> | <sup> <a href='https://aka.ms/xamarinpatternsebook'>**Download**  </a>  </sup> |

Download in other formats (**eReaders** like **MOBI**, **EPUB**) and other eBooks at the [.NET Architecture center](http://dot.net/architecture).

Send feedback to [dotnet-architecture-ebooks-feedback@service.microsoft.com](dotnet-architecture-ebooks-feedback@service.microsoft.com)

However, we encourage to download and review the [Architecting and Developing Microservices eBook](https://aka.ms/microservicesebook) because the architectural styles and architectural patterns and technologies explained in the guidance are using this reference application when explaining many pattern implementations, so you'll understand much better the context, design and decisions taken in the current architecture and internal designs.

## Overview of the application code
In this repo you can find a sample reference application that will help you to understand how to implement a microservice architecture based application using <b>.NET Core</b> and <b>Docker</b>.

The example business domain or scenario is based on an eShop or eCommerce which is implemented as a multi-container application. Each container is a microservice deployment (like the basket-microservice, catalog-microservice, ordering-microservice and the  identity-microservice) which are developed using ASP.NET Core running on .NET Core so they can run either on Linux Containers and Windows Containers.
The screenshot below shows the VS Solution structure for those microservices/containers and client apps.

- (*Recommended when getting started*) Open <b>eShopOnContainers-ServicesAndWebApps.sln</b> for a solution containing just the server-side projects related to the microservices and web applications.
- Open <b>eShopOnContainers-MobileApps.sln</b> for a solution containing just the client mobile app projects (Xamarin mobile apps only). It works independently based on mocks, too.
- Open <b>eShopOnContainers.sln</b> for a solution containing all the projects (All client apps and services).

<img src="img/vs-solution-structure.png">

Finally, those microservices are consumed by multiple client web and mobile apps, as described below.
<br>
<b>*MVC Application (ASP.NET Core)*</b>: It's an MVC application where you can find interesting scenarios on how to consume HTTP-based microservices from C# running in the server side, as it is a typical ASP.NET Core MVC application. Since it is a server-side application, access to other containers/microservices is done within the internal Docker Host network with its internal name resolution.
<img src="img/eshop-webmvc-app-screenshot.png">
<br>
<b>*SPA (Single Page Application)*</b>: Providing similar "eShop business functionality" but developed with Angular, Typescript and slightly using ASP.NET Core MVC. This is another approach for client web applications to be used when you want to have a more modern client behavior which is not behaving with the typical browser round-trip on every action but behaving like a Single-Page-Application which is more similar to a desktop app usage experience. The consumption of the HTTP-based microservices is done from TypeScript/JavaScript in the client browser, so the client calls to the microservices come from out of the Docker Host internal network (Like from your network or even from the Internet).
<img src="img/eshop-webspa-app-screenshot.png">
<br>
<b>*Xamarin Mobile App (For iOS, Android and Windows/UWP)*</b>: It is a client mobile app supporting the most common mobile OS platforms (iOS, Android and Windows/UWP). In this case, the consumption of the microservices is done from C# but running on the client devices, so out of the Docker Host internal network (Like from your network or even the Internet).

<img src="img/xamarin-mobile-App.png">

## Setting up your development environment for eShopOnContainers
### Visual Studio 2017 and Windows based
This is the more straightforward way to get started:
https://github.com/dotnet-architecture/eShopOnContainers/wiki/02.-Setting-eShopOnContainers-in-a-Visual-Studio-2017-environment

### CLI and Windows based
For those who prefer the CLI on Windows, using dotnet CLI, docker CLI and VS Code for Windows:
https://github.com/dotnet/eShopOnContainers/wiki/03.-Setting-the-eShopOnContainers-solution-up-in-a-Windows-CLI-environment-(dotnet-CLI,-Docker-CLI-and-VS-Code)

### CLI and Mac based
For those who prefer the CLI on a Mac, using dotnet CLI, docker CLI and VS Code for Mac:
https://github.com/dotnet-architecture/eShopOnContainers/wiki/04.-Setting-eShopOnContainer-solution-up-in-a-Mac,-VS-for-Mac-or-with-CLI-environment--(dotnet-CLI,-Docker-CLI-and-VS-Code)

## Orchestrators: Kubernetes and Service Fabric
See at the [Wiki](https://github.com/dotnet-architecture/eShopOnContainers/wiki) the posts on setup/instructions about how to deploy to Kubernetes or Service Fabric in Azure (although you could also deploy to any other cloud or on-premises).

## Sending feedback and pull requests
As mentioned, we'd appreciate your feedback, improvements and ideas.
You can create new issues at the issues section, do pull requests and/or send emails to **eshop_feedback@service.microsoft.com**

## Questions
[QUESTION] Answer +1 if the solution is working for you (Through VS2017 or CLI environment):
https://github.com/dotnet/eShopOnContainers/issues/107
