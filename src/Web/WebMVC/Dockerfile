FROM microsoft/aspnetcore:1.0.1
ENTRYPOINT ["dotnet", "WebMVC.dll"]
ARG source=.
WORKDIR /app
ENV ASPNETCORE_URLS http://*:80
EXPOSE 80
COPY $source .