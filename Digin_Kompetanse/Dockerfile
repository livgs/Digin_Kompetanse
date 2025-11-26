
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS dev
WORKDIR /app

COPY . .

CMD ["dotnet", "watch", "run", "--no-launch-profile", "--urls", "http://0.0.0.0:8080"]

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

# Restore dependencies
RUN dotnet restore

# Publish til en egen mappe
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Kopier publisert output fra build-stage
COPY --from=build /app/publish .

# Eksponer porten appen lytter på
EXPOSE 8080

# Start applikasjonen
ENTRYPOINT ["dotnet", "Digin_Kompetanse.dll"]
