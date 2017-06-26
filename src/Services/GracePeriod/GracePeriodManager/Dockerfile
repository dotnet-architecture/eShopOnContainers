FROM microsoft/aspnetcore:1.1.2
ARG source
WORKDIR /app
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "GracePeriodManager.dll"]