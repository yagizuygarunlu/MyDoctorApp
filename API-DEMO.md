# 🏥 MyDoctorApp API Demo

## 🚀 Quick Start Examples

### 1. Health Check
```bash
curl http://localhost:8080/health
```

**Response:**
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "postgres-db",
      "status": "Healthy",
      "duration": "00:00:00.0234567"
    }
  ]
}
```

### 2. Authentication
```bash
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@mydoctorapp.com",
    "password": "admin123"
  }'
```

### 3. Create Doctor Profile
```bash
curl -X POST http://localhost:8080/api/doctors \
  -H "Authorization: Bearer your_token" \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "Dr. Sarah Johnson",
    "speciality": "Cardiology",
    "summaryInfo": "Experienced cardiologist specializing in heart disease prevention",
    "email": "sarah.johnson@hospital.com",
    "phoneNumber": "+1-555-0123"
  }'
```

### 4. Book Appointment
```bash
curl -X POST http://localhost:8080/api/appointments \
  -H "Authorization: Bearer your_token" \
  -H "Content-Type: application/json" \
  -d '{
    "doctorId": 1,
    "patientName": "John Smith",
    "patientEmail": "john.smith@email.com",
    "appointmentDate": "2024-12-20T10:30:00Z",
    "notes": "Regular checkup"
  }'
```

## 📊 Vertical Slice Architecture in Action

### Appointment Feature Slice
```
Features/Appointments/
├── Commands/
│   ├── Create/CreateAppointmentCommand.cs    # ✅ Create + Validate + Handle
│   ├── Approve/ApproveAppointmentCommand.cs  # ✅ Approve workflow
│   └── Cancel/CancelAppointmentCommand.cs    # ✅ Cancellation logic
├── Queries/
│   └── GetAppointments/GetAppointmentsQuery.cs  # ✅ Filtered retrieval
└── AppointmentEndpoints.cs                   # ✅ API surface
```

**Each feature is completely self-contained!**

## 🐳 Docker One-Liner
```bash
git clone https://github.com/yourusername/MyDoctorApp && \
cd MyDoctorApp && \
docker-compose up -d
```

**That's it!** Your medical practice management system is running with:
- ✅ .NET 9 API on http://localhost:8080
- ✅ PostgreSQL 16 database  
- ✅ pgAdmin on http://localhost:5050
- ✅ Health checks configured
- ✅ JWT authentication ready

## 🎯 Why Developers Love This Project

- **🏗️ Modern Architecture**: Vertical Slice Architecture with CQRS
- **⚡ Latest Tech**: .NET 9, PostgreSQL 16, Docker
- **🔒 Security First**: JWT auth, input validation, rate limiting
- **📚 Well Documented**: Comprehensive README and guides
- **🧪 Tested**: Unit tests with high coverage
- **🌍 Production Ready**: Docker, health checks, logging

## 🤝 Join the Community

- ⭐ **Star this repo** if you find it useful
- 🍴 **Fork it** to customize for your needs  
- 💬 **Open issues** for questions or suggestions
- 🚀 **Contribute** to make healthcare software better

---
**Built with ❤️ for the developer community** 