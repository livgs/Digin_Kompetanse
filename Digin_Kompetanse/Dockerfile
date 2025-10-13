# SDK (for bygg og utvikling)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Kopier alt
COPY . ./

# Hvis vi kjører i prod → bygg ferdig appen
ARG BUILD_MODE=Release
RUN dotnet restore
RUN dotnet build -c $BUILD_MODE

# ==========================
# Runtime (for produksjon)
# ==========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/bin/$BUILD_MODE/net9.0 .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Digin_Kompetanse.dll"]
