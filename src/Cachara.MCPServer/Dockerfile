# =============================================================================
# Multi-stage Dockerfile for .NET 10 MCP Server (Console Application)
# =============================================================================

# Runtime stage - for running the console application
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

# Build stage - for compiling the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore dependencies
# Note: Copy project files first for better Docker layer caching
COPY ["Cachara.MCPServer/Cachara.MCPServer.csproj", "Cachara.MCPServer/"]
COPY ["Cachara.Shared/Cachara.Shared.csproj", "Cachara.Shared/"]

# Restore NuGet packages
RUN dotnet restore "Cachara.MCPServer/Cachara.MCPServer.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/Cachara.MCPServer"
RUN dotnet build "Cachara.MCPServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage - create deployment artifacts
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

# Publish the application
# /p:UseAppHost=false creates a framework-dependent deployment
RUN dotnet publish "Cachara.MCPServer.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# Final stage - create the runtime image
FROM base AS final
WORKDIR /app

# Copy published application from publish stage
COPY --from=publish /app/publish .

# Set the entry point for console application
ENTRYPOINT ["dotnet", "Cachara.MCPServer.dll"]

# Optional: Add health check for long-running console apps
# HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
#   CMD pgrep -f "Cachara.Content.MCPServer.dll" > /dev/null || exit 1
