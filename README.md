# 🏥 MyDoctorApp - Medical Practice Management System

[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-13+-blue.svg)](https://www.postgresql.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.txt)

## 📖 Overview
A comprehensive medical practice management system built with .NET 8, featuring appointment scheduling, patient management, doctor profiles, and treatment catalogs. Designed with Clean Architecture principles and modern development practices.

## ✨ Features
- 👨‍⚕️ **Doctor Management** - Comprehensive doctor profiles with specializations
- 👥 **Patient Management** - Secure patient information handling
- 📅 **Appointment System** - Booking, approval, and cancellation workflows
- 💊 **Treatment Catalog** - Services with detailed descriptions and FAQs
- ⭐ **Review System** - Patient feedback and ratings
- 🔐 **JWT Authentication** - Secure API access
- 🌍 **Multilingual Support** - English and Turkish localization
- 📊 **Structured Logging** - Comprehensive audit trails

## 🛠️ Tech Stack
- **Backend**: .NET 8, ASP.NET Core Web API
- **Database**: PostgreSQL with Entity Framework Core
- **Architecture**: Clean Architecture, CQRS with MediatR
- **Authentication**: JWT Bearer Tokens
- **Validation**: FluentValidation
- **Logging**: Serilog with structured logging
- **Testing**: xUnit, FluentAssertions

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL 13+
- Visual Studio 2022 or VS Code

### Installation
1. Clone the repository
2. Update connection string in `appsettings.json`
3. Run database migrations
4. Start the application

[Detailed setup instructions...]

## 📊 API Documentation
The API includes comprehensive endpoints for:
- Authentication & Authorization
- Doctor Management
- Patient Operations
- Appointment Scheduling
- Treatment Catalog
- Review System

## 🧪 Testing
Run the test suite: `dotnet test`

## 📝 License
This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.
