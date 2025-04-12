using WebApi.Domain.Common;
using WebApi.Domain.Enums;

namespace WebApi.Domain.Entities
{
    public class Appointment : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public int DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
        public string? Description { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public string? RejectionReason { get; set; }
        public Doctor Doctor { get; set; } = default!;
        public Patient Patient { get; set; } = default!;
    }
}
