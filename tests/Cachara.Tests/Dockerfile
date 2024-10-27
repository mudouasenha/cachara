FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["../tests/Cachara.Tests/Cachara.Tests.csproj", "../tests/Cachara.Tests/"]
COPY ["Cachara.Shared/Cachara.Shared.csproj", "Cachara.Shared/"]
COPY ["Cachara.Users.API/Cachara.Users.API.csproj", "Cachara.Users.API/"]
RUN dotnet restore "../tests/Cachara.Tests/Cachara.Tests.csproj"
COPY . .
WORKDIR "/src/../tests/Cachara.Tests"
RUN dotnet build "Cachara.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Cachara.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cachara.Tests.dll"]
