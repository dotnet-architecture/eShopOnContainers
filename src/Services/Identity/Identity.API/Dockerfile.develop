FROM microsoft/dotnet:2.2-sdk
ARG BUILD_CONFIGURATION=Debug
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
EXPOSE 80

WORKDIR /src
COPY ["src/Services/Identity/Identity.API/Identity.API.csproj", "src/Services/Identity/Identity.API/"]
COPY ["src/BuildingBlocks/WebHostCustomization/WebHost.Customization/WebHost.Customization.csproj", "src/BuildingBlocks/WebHostCustomization/WebHost.Customization/"]
RUN dotnet restore src/Services/Identity/Identity.API/Identity.API.csproj -nowarn:msb3202,nu1503
COPY . .
WORKDIR "/src/src/Services/Identity/Identity.API"
RUN dotnet build  --no-restore -c $BUILD_CONFIGURATION

ENTRYPOINT ["dotnet", "run", "--no-build", "--no-launch-profile", "-c", "$BUILD_CONFIGURATION", "--"]
