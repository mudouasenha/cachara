FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5200
EXPOSE 5201

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Cachara.Content.API/Cachara.Content.API.csproj", "Cachara.Content.API/"]
COPY ["Cachara.Shared/Cachara.Shared.csproj", "Cachara.Shared/"]
RUN dotnet restore "Cachara.Content.API/Cachara.Content.API.csproj"
COPY . .
WORKDIR "/src/Cachara.Content.API"
RUN dotnet build "Cachara.Content.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Cachara.Content.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cachara.Content.API.dll"]
