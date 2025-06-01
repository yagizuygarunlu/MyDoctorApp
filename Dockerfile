# Use the official .NET 9 runtime image as base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files and restore dependencies first (for better caching)
COPY ["WebApi/WebApi.csproj", "WebApi/"]
COPY ["WebApi.Tests/WebApi.Tests.csproj", "WebApi.Tests/"]
RUN dotnet restore "WebApi/WebApi.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/WebApi"

# Build the application
RUN dotnet build "WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage - runtime image
FROM base AS final
WORKDIR /app

# Create a non-root user for security
RUN addgroup --system --gid 1001 dotnetgroup && \
    adduser --system --uid 1001 --gid 1001 dotnetuser

# Copy published application
COPY --from=publish /app/publish .

# Create logs directory and set permissions
RUN mkdir -p /app/Logs && \
    chown -R dotnetuser:dotnetgroup /app

# Switch to non-root user
USER dotnetuser

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "WebApi.dll"] 