using MockQueryable.NSubstitute;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Domain.Entities;
using WebApi.Domain.Enums;
using WebApi.Features.Appointments.Queries.GetAppointments;

namespace WebApi.Tests.Features.Appointments.Queries
{
    public class GetAppointmentsQueryHandlerTests
    {
        private readonly IApplicationDbContext _context;
        public GetAppointmentsQueryHandlerTests()
        {
            _context = Substitute.For<IApplicationDbContext>();
        }

        [Fact]
        public async Task Handle_ShouldReturnAllAppointmenst()
        {
            // Arrange
            var appointments = new List<Appointment>
    {
        new Appointment
        {
            Id = Guid.NewGuid(),
            DoctorId = 1,
            PatientId = Guid.NewGuid(),
            Date = DateTime.UtcNow.AddDays(1),
            Status = AppointmentStatus.Approved,
            Description = "Routine Checkup",
  Doctor = new Doctor
                {
                    Id = 1,
                    FullName = "Dr. John Doe",
                    Speciality = "Cardiology",
                    SummaryInfo = "Expert",
                    Biography = "Biography",
                    Email = "doctor@example.com",
                    PhoneNumber = "+111111111",
                    ImageUrl = "https://example.com/image.jpg",

                },            Patient = new Patient
            {
                FullName = "Jane Doe",
                Email = "jane@example.com",
                PhoneNumber = "123-456-7890"
            }
        },
        new Appointment
        {
            Id = Guid.NewGuid(),
            DoctorId = 2,
            PatientId = Guid.NewGuid(),
            Date = DateTime.UtcNow.AddDays(2),
            Status = AppointmentStatus.Pending,
            Description = "Follow-up",
            Doctor = new Doctor {
                Id = 1,
                FullName = "Dr. Sarah Johnson",
                Speciality = "Cardiology",
                SummaryInfo = "Expert",
                Biography = "Biography",
                Email = "doctor@example.com",
                PhoneNumber = "+111111111",
                ImageUrl = "https://example.com/image.jpg"
            },
            Patient = new Patient
            {
                FullName = "John Doe",
                Email = "john@example.com",
                PhoneNumber = "987-654-3210"
            }
        }
    }.AsQueryable();
            var mockDbSet = appointments.BuildMockDbSet();

            _context.Appointments.Returns(mockDbSet);
            var handler = new GetAppointmentsQueryHandler(_context);

            // Act
            var result = await handler.Handle(new GetAppointmentsQuery(), CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();
            result.Data.Count.ShouldBe(2);
        }

    }
}
