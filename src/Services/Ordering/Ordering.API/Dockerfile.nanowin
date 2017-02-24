FROM microsoft/dotnet:1.1-runtime-nanoserver
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "Ordering.API.dll"]
