using WebApi.Domain.Common;

namespace WebApi.Domain.Entities
{
    public class Appointment : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public int DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public bool IsCancelled { get; set; }
        public Doctor Doctor { get; set; } = default!;
        public Patient Patient { get; set; } = default!;
    }
}
