FROM microsoft/aspnetcore:1.1.2
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "WebMVC.dll"]
