# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:latest AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY . .

WORKDIR /source
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/runtime:latest
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "FirefoxFontNotoConfigurator.dll"]