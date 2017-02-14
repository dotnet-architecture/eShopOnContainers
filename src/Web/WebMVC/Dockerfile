FROM microsoft/aspnetcore:1.1
ENTRYPOINT ["dotnet", "WebMVC.dll"]
ARG source=.
WORKDIR /app
ENV ASPNETCORE_URLS http://0.0.0.0:5100
COPY $source .
