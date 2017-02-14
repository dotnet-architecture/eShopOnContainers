FROM microsoft/aspnetcore:1.1
ENTRYPOINT ["dotnet", "Basket.API.dll"]
ARG source=.
WORKDIR /app
ENV ASPNETCORE_URLS http://*:5103
COPY $source .
