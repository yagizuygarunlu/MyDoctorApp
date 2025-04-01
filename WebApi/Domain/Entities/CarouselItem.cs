using WebApi.Domain.Common;

namespace WebApi.Domain.Entities
{
    public class CarouselItem : IEntity<int>
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string ImageUrl { get; set; }
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
