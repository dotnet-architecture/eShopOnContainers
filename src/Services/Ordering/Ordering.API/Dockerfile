FROM microsoft/aspnetcore:1.0.1
# Other .NET images
# FROM microsoft/dotnet:1.0.0-preview2-sdk
# FROM microsoft/dotnet:1.0.0-core
# FROM microsoft/aspnetcore
# FROM microsoft/dotnet:1.0.0-preview2-windowsservercore-sdk

# Entry point through the copied assembly 
ENTRYPOINT ["dotnet", "Ordering.API.dll"]
ARG source=.
WORKDIR /app
EXPOSE 80
COPY $source .


#################
# Entry point running the SDK --> Requires heavy Docker .NET image, not for production
# Restore NuGet packages
#RUN ["dotnet", "restore"]
# Build the .NET Core app
#RUN ["dotnet", "build"]
# Entrypoint
#ENTRYPOINT ["dotnet", "run"]
#################
