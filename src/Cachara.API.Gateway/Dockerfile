FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Cachara.API.Gateway/Cachara.API.Gateway.csproj", "Cachara.API.Gateway/"]
RUN dotnet restore "Cachara.API.Gateway/Cachara.API.Gateway.csproj"
COPY . .
WORKDIR "/src/Cachara.API.Gateway"
RUN dotnet build "Cachara.API.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Cachara.API.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cachara.API.Gateway.dll"]
