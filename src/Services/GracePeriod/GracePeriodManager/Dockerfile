FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "GracePeriodManager.dll"]