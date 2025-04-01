using WebApi.Domain.Common;

namespace WebApi.Domain.Entities
{
    public class PersonalLink : IEntity<int>
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int PlatformId { get; set; }
        public required string Url { get; set; }

        public Doctor Doctor { get; set; } = default!;
        public Platform Platform { get; set; } = default!;
    }
}
