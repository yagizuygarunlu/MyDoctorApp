using WebApi.Domain.Common;

namespace WebApi.Domain.Entities
{
    public class Patient: IEntity<Guid>
    {
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = [];
    }
}
