namespace WebApi.DTOs
{
    public record AppointmentDto(
        Guid Id,
        int DoctorId,
        string PatientName,
        string PatientEmail,
        string PatientPhone,
        DateTimeOffset AppointmentDate,
        string? Notes = null,
        bool IsCancelled = false
    );

}
