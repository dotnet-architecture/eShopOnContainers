FROM microsoft/aspnetcore:1.0.1

# Entry point through the copied assembly 
ENTRYPOINT ["dotnet", "Ordering.API.dll"]
ARG source=.
WORKDIR /app
EXPOSE 80
COPY $source .
