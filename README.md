<a href="https://dot.net/architecture">
   <img src="https://github.com/dotnet-architecture/eShopOnContainers/raw/dev/img/eshop_logo.png" alt="eShop logo" title="eShopOnContainers" align="right" height="60" />
</a>

# .NET Microservices Sample Reference Application

Sample .NET Core reference application, powered by Microsoft, based on a simplified microservices architecture and Docker containers.

![](img/eshop-webmvc-app-screenshot.png)

## Build Status (GitHub Actions)

| Image | Status | Image | Status |
| ------------- | ------------- | ------------- | ------------- |
| Web Status |  [![Web Status](https://github.com/dotnet-architecture/eShopOnContainers/workflows/webstatus/badge.svg?branch=dev)](https://github.com/dotnet-architecture/eShopOnContainers/actions?query=workflow%3Awebstatus) | Shopping Aggregator (Web) | [![Web Shopping Aggregator](https://github.com/dotnet-architecture/eShopOnContainers/workflows/webshoppingagg/badge.svg)](https://github.com/dotnet-architecture/eShopOnContainers/actions?query=workflow%3Awebshoppingagg) |
| Basket API | [![Basket API](https://github.com/dotnet-architecture/eShopOnContainers/workflows/basket-api/badge.svg?branch=dev)](https://github.com/dotnet-architecture/eShopOnContainers/actions?query=workflow%3Abasket-api) | Shopping Aggregator (Mobile) | [![Mobile Shopping Aggregator](https://github.com/dotnet-architecture/eShopOnContainers/workflows/mobileshoppingagg/badge.svg?branch=dev)](https://github.com/dotnet-architecture/eShopOnContainers/actions?query=workflow%3Amobileshoppingagg) |
| Catalog API | [![Catalog API](https://github.com/dotnet-architecture/eShopOnContainers/workflows/catalog-api/badge.svg)](https://github.com/dotnet-architecture/eShopOnContainers/actions?query=workflow%3Acatalog-api) | Web Client (MVC) | [![WebMVC Client](https://github.com/dotnet-architecture/eShopOnContainers/workflows/webmvc/badge.svg?branch=dev)](https://github.com/dotnet-architecture/eShopOnContainers/actions?query=workflow%3Awebmvc) |
|Identity API | [![Identity API](https://github.com/dotnet-architecture/eShopOnContainers/workflows/identity-api/badge.svg?branch=dev)](https://github.com/dotnet-architecture/eShopOnContainers/actions?query=workflow%3Aidentity-api) | Web Client (SPA) | [![WebSPA Client](https://github.com/dotnet-architecture/eShopOnContainers/workflows/webspa/badge.svg?branch=dev)](https://github.com/dotnet-architecture/eShopOnContainers/actions?query=workflow%3Awebspa) |
| Ordering API | [![Ordering API](https://github.com/dotnet-architecture/eShopOnContainers/workflows/ordering-api/badge.svg?branch=dev)](https://github.com/dotnet-architecture/eShopOnContainers/actions?query=workflow%3Aordering-api) | Webhooks Client | [![Webhooks demo client](https://github.com/dotnet-architecture/eShopOnContainers/workflows/webhooks-client/badge.svg)](https://github.com/dotnet-architecture/eShopOnContainers/actions?query=workflow%3Awebhooks-client) |
| Payment API | [![Payment API](https://github.com/dotnet-architecture/eShopOnContainers/workflows/payment-api/badge.svg?branch=dev)](https://github.com/dotnet-architecture/eShopOnContainers/actions?query=workflow%3Apayment-api) | Ordering SignalR | [![Ordering SignalR](https://github.com/dotnet-architecture/eShopOnContainers/workflows/ordering-signalrhub/badge.svg)](https://github.com/dotnet-architecture/eShopOnContainers/actions?query=workflow%3Aordering-signalrhub) | |

_**Dev** branch contains the latest **beta** code and their images are tagged with `:linux-dev` in our [Docker Hub](https://hub.docker.com/u/eshop)_

## Getting Started

Make sure you have [installed](https://docs.docker.com/docker-for-windows/install/) and [configured](https://github.com/dotnet-architecture/eShopOnContainers/wiki/Windows-setup#configure-docker) docker in your environment. After that, you can run the below commands from the **/src/** directory and get started with the `eShopOnContainers` immediately.

```powershell
docker-compose build
docker-compose up
```

You should be able to browse different components of the application by using the below URLs :

```
Web Status : http://host.docker.internal:5107/
Web MVC :  http://host.docker.internal:5100/
Web SPA :  http://host.docker.internal:5104/
```

>Note: If you are running this application in macOS then use `docker.for.mac.localhost` as DNS name in `.env` file and the above URLs instead of `host.docker.internal`.

Below are the other avenues to setup *eShopOnContainers*.

### Basic scenario

The basic scenario can be run locally using docker-compose, and also deployed to a local Kubernetes cluster. Refer to these Wiki pages to Get Started:


- [Visual Studio (F5 experience)](https://github.com/dotnet-architecture/eShopOnContainers/wiki/Windows-setup#optional---use-visual-studio)
- [Docker compose on windows](https://github.com/dotnet-architecture/eShopOnContainers/wiki/Windows-setup)
- [Docker compose on macOS](https://github.com/dotnet-architecture/eShopOnContainers/wiki/Mac-setup)
- [Local Kubernetes](https://github.com/dotnet-architecture/eShopOnContainers/wiki/Deploy-to-Local-Kubernetes)

### Advanced scenario

The Advanced scenario can be run only in a Kubernetes cluster. Currently this scenario is the same as a basic scenario with the following differences:

- [Deploy to AKS with a Service Mesh for resiliency](https://github.com/dotnet-architecture/eShopOnContainers/wiki/Deploy-to-Azure-Kubernetes-Service-(AKS))

In the future more features will be implemented in the advanced scenario.


## IMPORTANT NOTES!

**You can use either the latest version of Visual Studio or simply Docker CLI and .NET CLI for Windows, Mac and Linux**.

**Note for Pull Requests (PRs)**: We accept pull request from the community. When doing it, please do it onto the **DEV branch** which is the consolidated work-in-progress branch. Do not request it onto **master** branch.

**NEWS / ANNOUNCEMENTS**
Do you want to be up-to-date on .NET Architecture guidance and reference apps like eShopOnContainers? --> Subscribe by "WATCHING" this new GitHub repo: https://github.com/dotnet-architecture/News

## Updated for .NET 5

eShopOnContainers is updated to .NET 5 "wave" of technologies. Not just compilation but also new recommended code in EF Core, ASP.NET Core, and other new related versions with several significant changes.

**See more details in the [Release notes](https://github.com/dotnet-architecture/eShopOnContainers/wiki/Release-notes) wiki page**.

>**PLEASE** Read our [branch guide](./branch-guide.md) to know about our branching policy
>
> ### DISCLAIMER
>
> **IMPORTANT:** The current state of this sample application is **BETA**, because we are constantly evolving towards newly released technologies. Therefore, many areas could be improved and change significantly while refactoring the current code and implementing new features. Feedback with improvements and pull requests from the community will be highly appreciated and accepted.

### Architecture overview

This reference application is cross-platform at the server and client side, thanks to .NET 5 services capable of running on Linux or Windows containers depending on your Docker host, and to Xamarin for mobile apps running on Android, iOS or Windows/UWP plus any browser for the client web apps.
The architecture proposes a microservice oriented architecture implementation with multiple autonomous microservices (each one owning its own data/db) and implementing different approaches within each microservice (simple CRUD vs. DDD/CQRS patterns) using Http as the communication protocol between the client apps and the microservices and supports asynchronous communication for data updates propagation across multiple services based on Integration Events and an Event Bus (a light message broker, to choose between RabbitMQ or Azure Service Bus, underneath) plus other features defined at the [roadmap](https://github.com/dotnet-architecture/eShopOnContainers/wiki/Roadmap).

![](img/eshop_logo.png)
![](img/eShopOnContainers-architecture.png)

## Related documentation and guidance

You can find the related reference **Guide/eBook** focusing on **architecting and developing containerized and microservice based .NET Applications** (download link available below) which explains in detail how to develop this kind of architectural style (microservices, Docker containers, Domain-Driven Design for certain microservices) plus other simpler architectural styles, like monolithic apps that can also live as Docker containers.

There are also additional eBooks focusing on Containers/Docker lifecycle (DevOps, CI/CD, etc.) with Microsoft Tools, already published plus an additional eBook focusing on Enterprise Apps Patterns with Xamarin.Forms.
You can download them and start reviewing these Guides/eBooks here:

| Architecting & Developing | Containers Lifecycle & CI/CD | App patterns with Xamarin.Forms |
| ------------ | ------------|  ------------|
| [![](img/architecture-book-cover-large-we.png)](https://aka.ms/microservicesebook) | [![](img/devops-book-cover-large-we.png)](https://aka.ms/dockerlifecycleebook) | [![](img/xamarin-enterprise-patterns-ebook-cover-large-we.png)](https://aka.ms/xamarinpatternsebook) |
| <sup> <a href='https://aka.ms/microservicesebook'>**Download PDF**</a> </sup>  | <sup> <a href='https://aka.ms/dockerlifecycleebook'>**Download PDF** </a>  </sup> | <sup> <a href='https://aka.ms/xamarinpatternsebook'>**Download PDF**  </a>  </sup> |

For more free e-Books check out [.NET Architecture center](https://dot.net/architecture). If you have an e-book feedback, let us know by creating a new issue here: <https://github.com/dotnet-architecture/ebooks/issues>

## Are you new to **microservices** and **cloud-native development**? 
Take a look at the free course [Create and deploy a cloud-native ASP.NET Core microservice](https://docs.microsoft.com/en-us/learn/modules/microservices-aspnet-core/) on MS Learn.  This module explains microservices concepts, cloud-native technologies, and reduce the friction in getting started with `eShopOnContainers`.

## Read further

- [Explore the application](https://github.com/dotnet-architecture/eShopOnContainers/wiki/Explore-the-application)
- [Explore the code](https://github.com/dotnet-architecture/eShopOnContainers/wiki/Explore-the-code)

## Sending feedback and pull requests

Read the planned [Roadmap](https://github.com/dotnet-architecture/eShopOnContainers/wiki/Roadmap) within the Wiki for further info about possible new implementations and provide feedback at the [ISSUES section](https://github.com/dotnet/eShopOnContainers/issues) if you'd like to see any specific scenario implemented or improved. Also, feel free to discuss on any current issue.
