FROM microsoft/aspnetcore:1.1
ENTRYPOINT ["dotnet", "eShopOnContainers.WebSPA.dll"]
ARG source=.
WORKDIR /app
ENV ASPNETCORE_URLS http://0.0.0.0:5104
COPY $source .


