FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5300
EXPOSE 5301

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Cachara.GraphQL/Cachara.GraphQL.csproj", "Cachara.GraphQL/"]
RUN dotnet restore "Cachara.GraphQL/Cachara.GraphQL.csproj"
COPY . .
WORKDIR "/src/Cachara.GraphQL"
RUN dotnet build "Cachara.GraphQL.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Cachara.GraphQL.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cachara.GraphQL.dll"]
