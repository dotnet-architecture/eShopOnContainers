FROM microsoft/dotnet:1.0.0-preview2-sdk

#Lighter image if copying the compiled app and entrypoint as ENTRYPOINT dotnet Ordering.API.dll
#FROM microsoft/dotnet:1.0.0-core

# Set the Working Directory
WORKDIR /app

# Configure the listening port to 88
ENV ASPNETCORE_URLS http://*:88

# Open port exposed by Docker
EXPOSE 88/tcp

# Copy the app
COPY . /app

#################

# Restore NuGet packages
RUN ["dotnet", "restore"]

# Build the .NET Core app
RUN ["dotnet", "build"]

# Entrypoint
ENTRYPOINT ["dotnet", "run"]

#################

# Entry point through the copied assembly 
#ENTRYPOINT dotnet Ordering.API.dll