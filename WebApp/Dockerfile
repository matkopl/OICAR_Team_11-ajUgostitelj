# Base image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY WebApp/WebApp.csproj WebApp/
COPY WebAPI/WebAPI.csproj WebAPI/

# Restore dependencies
WORKDIR /src/WebApp
RUN dotnet restore

# Copy source code
COPY WebApp/. ./WebApp/
COPY WebAPI/. ./WebAPI/

# Build the WebApp (depends on WebAPI)
WORKDIR /src/WebApp
RUN dotnet build -c $BUILD_CONFIGURATION

# Publish the WebApp
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image for running the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAppFrontend.dll"]
