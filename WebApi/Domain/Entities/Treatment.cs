using WebApi.Domain.Common;

namespace WebApi.Domain.Entities
{
    public class Treatment : IEntity<int>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<TreatmentImage> Images { get; set; } = [];
        public ICollection<TreatmentFaq> Faqs { get; set; } = [];
    }
}
