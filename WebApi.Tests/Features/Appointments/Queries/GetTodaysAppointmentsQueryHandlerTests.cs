using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using NSubstitute;
using Shouldly;
using WebApi.Application.Common.Interfaces;
using WebApi.Domain.Entities;
using WebApi.Domain.Enums;
using WebApi.Features.Appointments.Queries.GetTodays;

public class GetTodaysAppointmentsQueryHandlerTests
{
    private readonly IApplicationDbContext _context;

    public GetTodaysAppointmentsQueryHandlerTests()
    {
        _context = Substitute.For<IApplicationDbContext>();
    }

    [Fact]
    public async Task Handle_ShouldReturnApprovedAppointmentsForToday()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;

        var appointments = new List<Appointment>
        {
            new Appointment
            {
                Id = Guid.NewGuid(),
                DoctorId = 1,
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
                   
                },
                Patient = new Patient
                {
                    Id = Guid.NewGuid(),
                    FullName = "Jane Doe",
                    Email = "jane@example.com",
                    PhoneNumber = "+222222222"
                },
                Date = today.AddHours(10),
                Status = AppointmentStatus.Approved,
                Description = "Routine Checkup"
            },
            new Appointment
            {
                Id =  Guid.NewGuid(),
                DoctorId = 2,
                Doctor = new Doctor
                {
                    Id = 2,
                    FullName = "Dr. Emily Smith",
                    Speciality = "Neurology",
                    SummaryInfo = "Expert",
                    Biography = "Biography",
                    Email = "doctor2@example.com",
                    PhoneNumber = "+333333333",
                    ImageUrl = "https://example.com/image2.jpg",
                   
                },
                Patient = new Patient
                {
                    Id =  Guid.NewGuid(),
                    FullName = "Tom Johnson",
                    Email = "tom@example.com",
                    PhoneNumber = "+444444444"
                },
                Date = today.AddDays(-1),
                Status = AppointmentStatus.Approved,
                Description = "Old appointment"
            }
        }.AsQueryable();

        var mockDbSet = appointments.BuildMockDbSet();

        _context.Appointments.Returns(mockDbSet);

        var handler = new GetTodaysAppointmentsQueryHandler(_context);

        // Act
        var result = await handler.Handle(new GetTodaysAppointmentsQuery(), CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Data.Count.ShouldBe(1);
        result.Data.First().DoctorName.ShouldBe("Dr. John Doe");
        result.Data.First().PatientName.ShouldBe("Jane Doe");
    }
}
