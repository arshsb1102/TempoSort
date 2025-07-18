# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0.412 AS build
WORKDIR /src

# Copy solution and restore
COPY NotificationService.sln ./
COPY NotificationService.API/*.csproj NotificationService.API/
COPY NotificationService.Business/*.csproj NotificationService.Business/
COPY NotificationService.DataAccess/*.csproj NotificationService.DataAccess/
COPY NotificationService.Models/*.csproj NotificationService.Models/

RUN dotnet restore NotificationService.sln

# Copy everything else
COPY . .

# Build and publish
RUN dotnet publish NotificationService.API/NotificationService.API.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0.412 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "NotificationService.API.dll"]
