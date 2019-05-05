FROM microsoft/dotnet:2.2-sdk
ARG BUILD_CONFIGURATION=Debug
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
EXPOSE 80

WORKDIR /src
COPY ["src/Web/WebMVC/WebMVC.csproj", "src/Web/WebMVC/"]
COPY ["src/BuildingBlocks/Devspaces.Support/Devspaces.Support.csproj", "src/BuildingBlocks/Devspaces.Support/"]
RUN dotnet restore "src/Web/WebMVC/WebMVC.csproj"
COPY . .
WORKDIR "/src/src/Web/WebMVC"
RUN dotnet build --no-restore "WebMVC.csproj" -c $BUILD_CONFIGURATION

ENTRYPOINT ["dotnet", "run", "--no-build", "--no-launch-profile", "-c", "$BUILD_CONFIGURATION", "--"]