# FROM microsoft/aspnet:1.0.0-rc1-update1
# COPY . /app
# WORKDIR /app
# RUN ["dnu", "restore"]
# EXPOSE 5104/tcp
# ENTRYPOINT ["dnx", "-p", "project.json", "web"]

FROM microsoft/aspnetcore:1.0.1
ENTRYPOINT ["dotnet", "eShopOnContainers.WebSPA.dll"]
ARG source=.
WORKDIR /app
ENV ASPNETCORE_URLS http://*:80
EXPOSE 80
COPY $source .
