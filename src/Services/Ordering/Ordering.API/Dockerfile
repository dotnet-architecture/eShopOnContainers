FROM microsoft/aspnetcore:1.1

# Entry point through the copied assembly 
ENTRYPOINT ["dotnet", "Ordering.API.dll"]
ARG source=.
WORKDIR /app
ENV ASPNETCORE_URLS http://*:5102
COPY $source .
