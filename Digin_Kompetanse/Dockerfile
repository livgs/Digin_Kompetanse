# --- BUILD STAGE ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore dependencies
RUN dotnet restore

# Publish to /app/publish
RUN dotnet publish -c Release -o /app/publish

# --- RUNTIME STAGE ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose port (for documentation; not required)
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "Digin_Kompetanse.dll"]
