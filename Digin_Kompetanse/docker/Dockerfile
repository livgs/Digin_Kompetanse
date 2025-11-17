FROM mcr.microsoft.com/dotnet/sdk:9.0 AS dev
WORKDIR /app

RUN dotnet tool install -g dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

COPY . ./

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY . ./
ARG BUILD_MODE=Release
RUN dotnet restore
RUN dotnet build -c $BUILD_MODE

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
ARG BUILD_MODE=Release
COPY --from=build /app/bin/$BUILD_MODE/net9.0 .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Digin_Kompetanse.dll"]
