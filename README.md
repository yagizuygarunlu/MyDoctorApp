# 🏥 MyDoctorApp - Medical Practice Management System

[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16+-blue.svg)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Supported-blue.svg)](https://www.docker.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.txt)

## 📖 Overview
A comprehensive medical practice management system built with .NET 9, featuring appointment scheduling, patient management, doctor profiles, and treatment catalogs. Designed with **Vertical Slice Architecture** and CQRS patterns, demonstrating modern development practices and enterprise-grade solutions.

> **Note**: This is a portfolio project showcasing modern .NET development, vertical slice architecture, and DevOps practices. Not intended for actual medical use without proper compliance certifications.

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
- **Architecture**: **Vertical Slice Architecture** with CQRS pattern
- **Messaging**: MediatR for request/response handling
- **Authentication**: JWT Bearer Tokens
- **Validation**: FluentValidation with localized error messages
- **Logging**: Serilog with structured logging and file/console outputs
- **Testing**: xUnit, FluentAssertions, NSubstitute
- **Containerization**: Docker & Docker Compose
- **Monitoring**: Health checks with database connectivity verification

## 🏗️ Architecture Overview

### Vertical Slice Architecture
This project implements **Vertical Slice Architecture**, organizing code by **features** rather than technical layers. Each feature contains all the necessary code to handle its specific business functionality.

#### Benefits:
- **Feature-Focused Organization**: Related code stays together
- **Reduced Coupling**: Features are self-contained and independent
- **Easy Navigation**: Find all appointment logic in the Appointments folder
- **Simplified Testing**: Each feature can be tested in isolation
- **Team Scalability**: Different teams can work on different features independently

#### Feature Structure:
```
Features/
├── Appointments/           # Complete appointment management
│   ├── Commands/          # State-changing operations
│   │   ├── Create/        # Create appointment + validation + handler
│   │   ├── Approve/       # Approve appointment logic
│   │   ├── Cancel/        # Cancellation workflow
│   │   └── Reject/        # Rejection with reason
│   ├── Queries/           # Data retrieval operations
│   │   ├── GetAppointments/   # List appointments with filtering
│   │   └── GetTodays/         # Today's appointments
│   └── AppointmentEndpoints.cs # API endpoints for this feature
├── Doctors/               # Doctor management feature slice
├── Patients/              # Patient management feature slice
└── Treatments/            # Treatment catalog feature slice
```

### CQRS Pattern Integration
Combined with **Command Query Responsibility Segregation (CQRS)**:
- **Commands**: Handle business operations (Create, Update, Delete)
- **Queries**: Handle data retrieval optimized for specific use cases
- **Handlers**: Contain business logic for each operation
- **Validators**: FluentValidation for each command/query

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
cp env.example .env

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

> 📋 **Want to see the API in action?** Check out our [Live API Demo](API-DEMO.md) with ready-to-run examples!

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

### Vertical Slice Organization
```
MyDoctorApp/
├── WebApi/                          # Main API project
│   ├── Features/                   # 🎯 FEATURE-BASED ORGANIZATION
│   │   ├── Appointments/           # Complete appointment management slice
│   │   │   ├── Commands/          # Business operations
│   │   │   │   ├── Create/        # CreateAppointmentCommand + Handler + Validator
│   │   │   │   ├── Approve/       # ApproveAppointmentCommand + Handler
│   │   │   │   ├── Cancel/        # CancelAppointmentCommand + Handler
│   │   │   │   └── Reject/        # RejectAppointmentCommand + Handler
│   │   │   ├── Queries/           # Data retrieval operations
│   │   │   │   ├── GetAppointments/   # GetAppointmentsQuery + Handler
│   │   │   │   └── GetTodays/         # GetTodaysAppointmentsQuery + Handler
│   │   │   └── AppointmentEndpoints.cs # API endpoint definitions
│   │   ├── Doctors/               # Doctor management feature slice
│   │   │   ├── Commands/          # Create, Update, Delete, Reactivate
│   │   │   ├── Queries/           # Doctor-specific queries
│   │   │   └── DoctorEndpoints.cs # Doctor API endpoints
│   │   ├── Patients/              # Patient management feature slice
│   │   ├── Treatments/            # Treatment catalog feature slice
│   │   ├── Reviews/               # Review system feature slice
│   │   ├── Auth/                  # Authentication feature slice
│   │   └── TreatmentFaqs/         # Treatment FAQ management
│   ├── Domain/                    # Domain entities and business models
│   │   ├── Entities/              # Core business entities
│   │   ├── Enums/                 # Domain enumerations
│   │   └── ValueObjects/          # Domain value objects
│   ├── Infrastructure/            # Data access and external services
│   │   ├── Persistence/           # Entity Framework configuration
│   │   └── Services/              # External service implementations
│   ├── Common/                    # Shared utilities and cross-cutting concerns
│   │   ├── Localization/          # Multi-language support
│   │   ├── Results/               # Result pattern implementation
│   │   └── Extensions/            # Extension methods
│   └── Migrations/                # EF Core database migrations
├── WebApi.Tests/                  # Unit and integration tests
├── docker-compose.yml             # Docker configuration
├── Dockerfile                     # Container definition
└── DOCKER_SETUP.md               # Docker documentation
```

### Key Architectural Principles

#### 1. **Feature Cohesion**
Each feature folder contains everything needed for that business capability:
- Commands (business operations)
- Queries (data retrieval)
- Validators (input validation)
- Handlers (business logic)
- Endpoints (API surface)

#### 2. **Independence**
Features are designed to be independent of each other, reducing coupling and enabling:
- Independent deployment (if needed)
- Team autonomy
- Easier testing and maintenance

#### 3. **Consistent Patterns**
Each feature follows the same organizational pattern:
```
FeatureName/
├── Commands/
│   └── OperationName/
│       └── OperationNameCommand.cs     # Command + Handler + Validator
├── Queries/
│   └── QueryName/
│       └── QueryNameQuery.cs           # Query + Handler + Response
└── FeatureNameEndpoints.cs             # API endpoints
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

### Vertical Slice Architecture Benefits:
- **Feature-Centric Organization**: All related code for a business feature is co-located
- **Reduced Coupling**: Features are independent and self-contained
- **Improved Maintainability**: Changes to one feature don't affect others
- **Team Scalability**: Multiple teams can work on different features simultaneously
- **Easier Onboarding**: New developers can understand one feature at a time

### CQRS Implementation:
- **Commands**: Handle state-changing operations (Create, Update, Delete)
- **Queries**: Handle read operations with optimized data transfer
- **MediatR**: Decoupled request/response handling across feature slices
- **Validation**: FluentValidation for robust input validation per operation

### Design Patterns Used:
- ✅ **Vertical Slice Architecture**: Feature-based organization
- ✅ **CQRS Pattern**: Command/Query separation
- ✅ **Repository Pattern**: Data access abstraction
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

- Built with ❤️ using .NET 9 and Vertical Slice Architecture
- Inspired by real-world medical practice management needs
- Designed to showcase modern enterprise-level software architecture patterns

---
**⭐ If you find this project helpful, please give it a star!**

## 🌟 **Found This Useful?**

If this project helped you learn about Vertical Slice Architecture, modern .NET development, or healthcare software design, please consider:

- ⭐ **Starring this repository** - it really helps!
- 🍴 **Forking it** to build your own medical practice system
- 💬 **Opening an issue** with feedback or questions
- 🚀 **Contributing** to make it even better
- 📢 **Sharing it** with other developers who might find it useful

**Your support helps me create more high-quality open-source projects!**

---

### 🤝 Connect & Follow
- **LinkedIn**: [Connect with me](https://linkedin.com/in/yourprofile) for updates
- **Twitter**: [@yourusername](https://twitter.com/yourusername) - Follow for dev tips
- **Portfolio**: [yourwebsite.com](https://yourwebsite.com) - Check out my other projects

*Building the future of healthcare software, one commit at a time.* 💻🏥
