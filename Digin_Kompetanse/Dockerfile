FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Digin_Kompetanse/Digin_Kompetanse.csproj", "Digin_Kompetanse/"]
RUN dotnet restore "Digin_Kompetanse/Digin_Kompetanse.csproj"
COPY . .
WORKDIR "/src/Digin_Kompetanse"
RUN dotnet build "./Digin_Kompetanse.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Digin_Kompetanse.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Digin_Kompetanse.dll"]
