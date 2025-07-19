# Stage 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0.401 AS build

WORKDIR /app
COPY . ./

RUN dotnet restore NotificationService.sln
RUN dotnet publish NotificationService.sln -c Release -o /app/out

# Stage 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0.4 AS runtime

WORKDIR /app
COPY --from=build /app/out ./

EXPOSE 80
ENTRYPOINT ["dotnet", "NotificationService.API.dll"]
