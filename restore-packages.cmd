for /R %%f in (*.csproj) do dotnet restore --no-dependencies %%f
