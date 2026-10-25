# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Restore dependencies
COPY TemperatureApi.csproj ./
RUN dotnet restore

# Copy the source code and publish
COPY . .
RUN dotnet publish -c Release -o /app --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app .

# The port the app runs on
ENV ASPNETCORE_HTTP_PORTS=8081
EXPOSE 8081

ENTRYPOINT ["dotnet", "TemperatureApi.dll"]
