FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5100
EXPOSE 5101

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Cachara.Users.API/Cachara.Users.API.csproj", "Cachara.Users.API/"]
COPY ["Cachara.Shared/Cachara.Shared.csproj", "Cachara.Shared/"]
RUN dotnet restore "Cachara.Users.API/Cachara.Users.API.csproj"
COPY . .
WORKDIR "/src/Cachara.Users.API"
RUN dotnet build "Cachara.Users.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Cachara.Users.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cachara.Users.API.dll"]
