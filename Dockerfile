FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Backend/Backend.csproj", "Backend/"]
WORKDIR /src/Backend
RUN dotnet restore

COPY Backend/. .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Backend.dll"]