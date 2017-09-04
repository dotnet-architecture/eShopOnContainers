# Running the Catalog Editor WebForms application

You can run the catalog editor Web Forms application locally, using
hard coded catalog data, or in a set of Windows based Docker Containers,
using the catalog microservice.

To run locally, you edit the [web.config](web.config) file to set the
`usefake` flag to true:

```xml
  <appSettings>
    <add key="usefake" value="true" />
    <add key ="CatalogURL" value="http://catalog.api:5101" />
  </appSettings>
```

Set that value to `false` to run with the full catalog microservice.

Then, set the startup project in Visual Studio to the *Catalog.WebForms* project. Build and run, and test the application.

## Configure Windows Containers

You'll need to install and configure
[Docker for Windows](https://docs.docker.com/docker-for-windows/install/) to run
this project in Docker containers.

The Catalog.WebForms project uses the full .NET framework, so will only run in Windows based Docker containers. Before running in *Docker for Windows*, make sure you
are running Docker with Windows containers configured. Right-click on the *Docker for Windows* node, and if "switch to Windows Containers" is displayed, click that. If "switch to Linux containers" is displayed, you are already running
with Windows containers.

## Update Configured IP address

There is a current limitation when running with
Windows containers where *localhost* doesn't route from the Docker host to a container.

There are two workarounds for this. You need to find the IP address of the host machine to access
the external address of the services, or you can use the IP address of the host running the container
and its exposed port. This project uses several containers, so the first method is simpler.

To find your machine's IP address, run `ipconfig`. Find the IPv4 address
for your machine. In the GitHub example, it is configured for a home
network, and the address is `192.168.1.103`.

Edit the *.env* file and set the `ESHOP_EXTERNAL_DNS_NAME_OR_IP` to
your machine's address.

```yml
ESHOP_EXTERNAL_DNS_NAME_OR_IP=192.168.1.103
```

For more details on the issue and workarounds, see [this StackOverflow post and answer](http://stackoverflow.com/questions/43769806/docker-for-windows-cannot-access-service-on-exposed-port-in-windows-container-m/43770786)

## Building for Docker

The Visual Studio tooling to build and start the docker application isn't
available yet for Windows containers. We'll update this example when it
is.

In the meantime, you can build all the .NET Core based projects from the 
command line.

From the *WebForms.Catalog* direcotry, type the following two commands:

```powershell
dotnet restore Catalog.WebForms.sln
dotnet publish Catalog.WebForms.sln -c Release -o ./obj/Docker/publish
```

You will get errors from the WebForms based project, but you can ignore those.
You now must build the *Catalog.WebForms* project in Visual Studio, and
publish it. There is a publish profile that publishes the WebForms project
to the correct directory, *./obj/Docker/publish*.

Once you've built the project, you build the Docker images and start the application with these two commands:

```powershell
docker-compose -f docker-compose.yml -f docker-compose.override.yml build
docker-compose -f docker-compose.yml -f docker-compose.override.yml up
```

When you run the second command, the Docker host will run the application.
Open a browser, and navigate to the IP address you retrieved from *ipconfig*
earlier. (In the example, it would be http://192.168.1.103)

## Roadmap and updates

As the tooling matures, we'll update the application and these instructions
with simpler steps to run the application.
