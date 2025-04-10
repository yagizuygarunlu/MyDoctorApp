using WebApi.Domain.Common;
using WebApi.Domain.ValueObjects;

namespace WebApi.Domain.Entities
{
    public class Doctor : IEntity<int>
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Speciality { get; set; }
        public required string SummaryInfo { get; set; }
        public required string Biography { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        public Address Address { get; set; } = default!;
        public ICollection<PersonalLink> PersonalLinks { get; set; } = [];
        public ICollection<Appointment> Appointments { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
    }
}
