﻿# =============================================================================
# Multi-stage Dockerfile for .NET 10 API
# =============================================================================

# Runtime stage - for running the application
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

# Expose ports for HTTP and HTTPS
EXPOSE 5200
EXPOSE 5201

# Build stage - for compiling the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore dependencies
# Note: Copy project files first for better Docker layer caching
COPY ["Cachara.Content.API/Cachara.Content.API.csproj", "Cachara.Content.API/"]
COPY ["Cachara.Shared/Cachara.Shared.csproj", "Cachara.Shared/"]

# Restore NuGet packages
RUN dotnet restore "Cachara.Content.API/Cachara.Content.API.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/Cachara.Content.API"
RUN dotnet build "Cachara.Content.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage - create deployment artifacts
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

# Publish the application
# /p:UseAppHost=false creates a framework-dependent deployment
RUN dotnet publish "Cachara.Content.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# Final stage - create the runtime image
FROM base AS final
WORKDIR /app

# Copy published application from publish stage
COPY --from=publish /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "Cachara.Content.API.dll"]
