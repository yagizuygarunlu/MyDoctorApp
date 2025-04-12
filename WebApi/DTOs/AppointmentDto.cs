using WebApi.Domain.Enums;

namespace WebApi.DTOs
{
    public record AppointmentDto(
        Guid Id,
        int DoctorId,
        string DoctorName,
        string PatientName,
        string PatientEmail,
        string PatientPhone,
        DateTimeOffset AppointmentDate,
        string? Notes = null,
        AppointmentStatus Status = AppointmentStatus.Pending
    );

}
