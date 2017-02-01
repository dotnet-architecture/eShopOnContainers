FROM microsoft/aspnetcore:1.1
ENTRYPOINT ["dotnet", "Identity.API.dll"]
ARG source=.
WORKDIR /app
ENV ASPNETCORE_URLS http://*:5105
COPY $source .
