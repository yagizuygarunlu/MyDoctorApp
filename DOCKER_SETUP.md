# 🐳 Docker Setup Guide for MyDoctorApp

This guide covers how to run MyDoctorApp using Docker and Docker Compose with .NET 9 and PostgreSQL 16.

## 📋 Prerequisites

- Docker Desktop (Windows/Mac) or Docker Engine (Linux)
- Docker Compose v2+
- Git (to clone the repository)

## 🚀 Quick Start

### 1. Clone the Repository
```bash
git clone <your-repo-url>
cd MyDoctorApp
```

### 2. Production Deployment
```bash
# Build and start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Access the application
# API: http://localhost:8080
# pgAdmin: http://localhost:5050 (admin@mydoctorapp.com / admin123)
```

### 3. Development Environment
```bash
# Use development override (different ports and settings)
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d

# Access the application
# API: http://localhost:5000
# PostgreSQL: localhost:5433
```

## 📁 Docker Files Overview

### `Dockerfile`
- **Multi-stage build** for optimized image size
- **Non-root user** for security
- **Health checks** included
- **Optimized for .NET 9**

### `docker-compose.yml`
- **Production configuration**
- **PostgreSQL 16** with health checks
- **Networking** between services
- **Persistent volumes** for data
- **Optional pgAdmin** for database management

### `docker-compose.override.yml`
- **Development configuration**
- **Hot reload** support (when using development image)
- **Different ports** to avoid conflicts
- **Development database**

## ⚙️ Configuration

### Environment Variables

The application uses the following environment variables in Docker:

```yaml
# Application Configuration
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080;https://+:8081

# Database Connection
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=MyDoctorDb;Username=postgres;Password=MySecurePassword123!

# JWT Configuration
Jwt__Key=YourSuperSecretKeyForJWTTokenGenerationMustBe32CharsLong!
Jwt__Issuer=MyDoctorApp
Jwt__Audience=MyDoctorAppUsers
```

### Security Notes

> ⚠️ **Important**: The default passwords and JWT keys in the Docker files are for development only. 
> **Always change these in production!**

For production:
1. Use environment files (`.env`)
2. Use Docker secrets
3. Use a secure key management system

## 🛠️ Development Workflow

### Building the Image
```bash
# Build the application image
docker build -t mydoctorapp:latest .

# Build with specific tag
docker build -t mydoctorapp:v1.0.0 .
```

### Database Operations
```bash
# Access PostgreSQL container
docker exec -it mydoctorapp-postgres psql -U postgres -d MyDoctorDb

# Run database migrations (if needed)
docker exec -it mydoctorapp-api dotnet ef database update

# View database logs
docker logs mydoctorapp-postgres
```

### Application Debugging
```bash
# View application logs
docker logs mydoctorapp-api -f

# Access application container
docker exec -it mydoctorapp-api /bin/bash

# Run health check manually
curl http://localhost:8080/health
```

## 📊 Service Management

### Starting Services
```bash
# Start all services
docker-compose up -d

# Start specific service
docker-compose up -d postgres

# Start with build
docker-compose up -d --build
```

### Stopping Services
```bash
# Stop all services
docker-compose down

# Stop and remove volumes
docker-compose down -v

# Stop and remove everything
docker-compose down -v --rmi all
```

### Scaling Services
```bash
# Scale API service (behind a load balancer)
docker-compose up -d --scale mydoctorapp-api=3
```

## 🔍 Monitoring & Health Checks

### Health Check Endpoints
- **Application Health**: `http://localhost:8080/health`
- **Swagger/OpenAPI**: `http://localhost:8080/swagger` (development only)

### Health Check Response
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "postgres-db",
      "status": "Healthy",
      "duration": "00:00:00.0234567"
    },
    {
      "name": "self",
      "status": "Healthy",
      "duration": "00:00:00.0001234"
    }
  ],
  "duration": "00:00:00.0456789"
}
```

### Container Health Status
```bash
# Check container health
docker ps

# View health check logs
docker inspect mydoctorapp-api | grep -A 10 -B 5 Health
```

## 🗄️ Database Management

### pgAdmin (Optional)
Include pgAdmin in your deployment:

```bash
# Start with pgAdmin
docker-compose --profile tools up -d

# Access pgAdmin at http://localhost:5050
# Email: admin@mydoctorapp.com
# Password: admin123
```

### Database Backup & Restore
```bash
# Backup database
docker exec mydoctorapp-postgres pg_dump -U postgres MyDoctorDb > backup.sql

# Restore database
docker exec -i mydoctorapp-postgres psql -U postgres MyDoctorDb < backup.sql
```

## 🌐 Production Deployment

### Using Docker Compose in Production
```bash
# Production deployment with custom env file
docker-compose --env-file .env.production up -d

# Use production-specific compose file
docker-compose -f docker-compose.prod.yml up -d
```

### Environment File (.env.production)
```env
# Database
POSTGRES_PASSWORD=your-very-secure-password
MYDOCTORAPP_DB_CONNECTION=Host=postgres;Port=5432;Database=MyDoctorDb;Username=postgres;Password=your-very-secure-password

# JWT
MYDOCTORAPP_JWT_KEY=your-super-secret-jwt-key-32-characters-minimum
MYDOCTORAPP_JWT_ISSUER=MyDoctorApp
MYDOCTORAPP_JWT_AUDIENCE=MyDoctorAppUsers

# Application
ASPNETCORE_ENVIRONMENT=Production
```

## 🔧 Troubleshooting

### Common Issues

1. **Port Conflicts**
   ```bash
   # Find what's using the port
   netstat -ano | findstr :8080  # Windows
   lsof -i :8080                 # Linux/Mac
   
   # Change ports in docker-compose.yml
   ports:
     - "8081:8080"  # Use different external port
   ```

2. **Database Connection Issues**
   ```bash
   # Check if PostgreSQL is running
   docker logs mydoctorapp-postgres
   
   # Test connection from API container
   docker exec mydoctorapp-api ping postgres
   ```

3. **Permission Issues**
   ```bash
   # Fix log directory permissions
   sudo chown -R 1001:1001 ./WebApi/Logs
   ```

4. **Memory Issues**
   ```bash
   # Check container resource usage
   docker stats
   
   # Increase Docker Desktop memory limit
   # Docker Desktop > Settings > Resources > Advanced
   ```

### Useful Commands
```bash
# View all containers
docker ps -a

# View images
docker images

# Clean up unused resources
docker system prune -a

# View Docker Compose configuration
docker-compose config

# Follow logs for all services
docker-compose logs -f
```

## 📈 Performance Optimization

### Image Size Optimization
- Multi-stage builds ✅
- .dockerignore file ✅
- Minimal base images ✅
- Non-root user ✅

### Runtime Optimization
```yaml
# Add resource limits
services:
  mydoctorapp-api:
    deploy:
      resources:
        limits:
          memory: 512M
          cpus: '0.5'
        reservations:
          memory: 256M
          cpus: '0.25'
```

## 🔐 Security Best Practices

- ✅ Non-root user in containers
- ✅ Health checks implemented
- ✅ Environment variable configuration
- ✅ Network isolation
- ⚠️ Use secrets management in production
- ⚠️ Regular security updates
- ⚠️ SSL/TLS certificates for HTTPS

## 📚 Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [.NET Docker Guide](https://docs.microsoft.com/en-us/dotnet/core/docker/)
- [PostgreSQL Docker Hub](https://hub.docker.com/_/postgres) 