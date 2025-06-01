# 🏥 MyDoctorApp - Medical Practice Management System

[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16+-blue.svg)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Supported-blue.svg)](https://www.docker.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.txt)

## 📖 Overview
A comprehensive medical practice management system built with .NET 9, featuring appointment scheduling, patient management, doctor profiles, and treatment catalogs. Designed with Clean Architecture and CQRS patterns, demonstrating modern development practices and enterprise-grade solutions.

> **Note**: This is a portfolio project showcasing modern .NET development, clean architecture, and DevOps practices. Not intended for actual medical use without proper compliance certifications.

## ✨ Features
- 👨‍⚕️ **Doctor Management** - Comprehensive doctor profiles with specializations and personal links
- 👥 **Patient Management** - Secure patient information handling with data validation
- 📅 **Appointment System** - Complete booking, approval, and cancellation workflows
- 💊 **Treatment Catalog** - Services with detailed descriptions, images, and FAQs
- ⭐ **Review System** - Patient feedback and ratings management
- 🔐 **JWT Authentication** - Secure API access with role-based authorization
- 🌍 **Multilingual Support** - English and Turkish localization
- 📊 **Structured Logging** - Comprehensive audit trails with Serilog
- 🐳 **Docker Support** - Complete containerization with PostgreSQL
- 🏥 **Health Checks** - Application and database monitoring endpoints

## 🛠️ Tech Stack
- **Backend**: .NET 9, ASP.NET Core Web API
- **Database**: PostgreSQL 16+ with Entity Framework Core
- **Architecture**: Clean Architecture, Vertical Slice Architecture, CQRS with MediatR
- **Authentication**: JWT Bearer Tokens
- **Validation**: FluentValidation with localized error messages
- **Logging**: Serilog with structured logging and file/console outputs
- **Testing**: xUnit, FluentAssertions, NSubstitute
- **Containerization**: Docker & Docker Compose
- **Monitoring**: Health checks with database connectivity verification

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- PostgreSQL 16+ (or Docker)
- Visual Studio 2022 / VS Code / JetBrains Rider
- Docker & Docker Compose (optional but recommended)

### 🐳 Option 1: Docker Setup (Recommended)

**Quick Start:**
```bash
# Clone the repository
git clone https://github.com/yourusername/MyDoctorApp.git
cd MyDoctorApp

# Copy environment template and customize if needed
cp .env.example .env

# Start with Docker Compose
docker-compose up -d

# View logs
docker-compose logs -f
```

**Access Points:**
- **API**: http://localhost:8080
- **Health Check**: http://localhost:8080/health  
- **pgAdmin**: http://localhost:5050 (admin@mydoctorapp.com / admin123)

For detailed Docker instructions, see [DOCKER_SETUP.md](DOCKER_SETUP.md).

### 💻 Option 2: Local Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/MyDoctorApp.git
   cd MyDoctorApp
   ```

2. **Setup PostgreSQL Database**
   ```sql
   -- Create development database
   CREATE DATABASE "MyDoctorDB_Dev";
   ```

3. **Configure Application Settings**
   
   Update `WebApi/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=127.0.0.1;Port=5432;Database=MyDoctorDB_Dev;User Id=postgres;Password=your_local_password;"
     },
     "Jwt": {
       "Key": "YourLocalDevelopmentKey32CharsMinimum!"
     }
   }
   ```

4. **Install Dependencies & Run Migrations**
   ```bash
   cd WebApi
   dotnet restore
   dotnet ef database update
   ```

5. **Start the Application**
   ```bash
   dotnet run
   ```

6. **Access the Application**
   - API: http://localhost:5000
   - Swagger: http://localhost:5000/swagger (development only)
   - Health Check: http://localhost:5000/health

## 🔑 Authentication & Authorization

The API uses JWT authentication with the following default admin account:
- **Email**: Check `DataSeeder.cs` for seeded credentials
- **Endpoint**: `POST /api/auth/login`

Example login request:
```bash
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"admin_password"}'
```

Include the JWT token in subsequent requests:
```bash
curl -H "Authorization: Bearer your_jwt_token" http://localhost:8080/api/doctors
```

## 📁 Project Structure
```
MyDoctorApp/
├── WebApi/                      # Main API project
│   ├── Features/               # Feature-based organization (Vertical Slice)
│   │   ├── Appointments/       # Appointment management
│   │   ├── Doctors/           # Doctor management  
│   │   ├── Patients/          # Patient management
│   │   ├── Treatments/        # Treatment catalog
│   │   ├── Reviews/           # Review system
│   │   └── Auth/              # Authentication
│   ├── Domain/                # Domain entities and business logic
│   │   ├── Entities/          # Core business entities
│   │   ├── Enums/             # Domain enumerations
│   │   └── ValueObjects/      # Domain value objects
│   ├── Infrastructure/        # Data access and external services
│   │   ├── Persistence/       # Entity Framework configuration
│   │   └── Services/          # External service implementations
│   ├── Common/                # Shared utilities and extensions
│   │   ├── Localization/      # Multi-language support
│   │   ├── Results/           # Result pattern implementation
│   │   └── Extensions/        # Extension methods
│   └── Migrations/            # EF Core database migrations
├── WebApi.Tests/              # Unit and integration tests
├── docker-compose.yml         # Docker configuration
├── Dockerfile                 # Container definition
└── DOCKER_SETUP.md           # Docker documentation
```

## 🧪 Testing
```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run with coverage (requires coverage tools)
dotnet test --collect:"XPlat Code Coverage"
```

## 📊 API Documentation

### Core Endpoints:
- **Authentication**: `/api/auth/login`
- **Doctors**: `/api/doctors` (CRUD operations)
- **Patients**: `/api/patients` (CRUD operations)  
- **Appointments**: `/api/appointments` (Booking, approval, cancellation)
- **Treatments**: `/api/treatments` (Service catalog)
- **Reviews**: `/api/reviews` (Patient feedback)
- **Health**: `/health` (Application health status)

### API Features:
- ✅ RESTful design principles
- ✅ Consistent error handling
- ✅ Input validation with localized messages  
- ✅ Rate limiting protection
- ✅ CORS configuration
- ✅ OpenAPI/Swagger documentation (development)

## 🌐 Deployment

### Environment Variables for Production:
```bash
# Database
ConnectionStrings__DefaultConnection="Host=your-host;Database=MyDoctorDb;Username=user;Password=secure-password"

# JWT Authentication
Jwt__Key="YourSecureJwtKey32CharactersMinimum!"
Jwt__Issuer="MyDoctorApp"
Jwt__Audience="MyDoctorAppUsers"

# Application
ASPNETCORE_ENVIRONMENT="Production"
```

### Security Considerations:
- 🔒 Never commit secrets to version control
- 🔒 Use environment variables or secret managers in production
- 🔒 Rotate JWT keys regularly
- 🔒 Use HTTPS in production
- 🔒 Implement proper CORS policies
- 🔒 Regular security updates

## 📋 Architecture Highlights

### Clean Architecture Benefits:
- **Separation of Concerns**: Clear boundaries between layers
- **Dependency Inversion**: Dependencies point inward toward business logic
- **Testability**: Easy to unit test business logic in isolation
- **Maintainability**: Changes in one layer don't affect others

### CQRS Implementation:
- **Commands**: Handle state-changing operations (Create, Update, Delete)
- **Queries**: Handle read operations with optimized data transfer
- **MediatR**: Decoupled request/response handling
- **Validation**: FluentValidation for robust input validation

### Design Patterns Used:
- ✅ **Repository Pattern**: Data access abstraction
- ✅ **Unit of Work**: Transaction management
- ✅ **Result Pattern**: Consistent error handling
- ✅ **Options Pattern**: Configuration management
- ✅ **Dependency Injection**: Loose coupling

## 🤝 Contributing

This is a portfolio project, but feedback and suggestions are welcome!

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

## 🙏 Acknowledgments

- Built with ❤️ using .NET 9 and modern development practices
- Inspired by real-world medical practice management needs
- Designed to showcase enterprise-level software architecture

---
**⭐ If you find this project helpful, please give it a star!**
