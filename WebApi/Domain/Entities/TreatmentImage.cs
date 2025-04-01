using WebApi.Domain.Common;

namespace WebApi.Domain.Entities
{
    public class TreatmentImage: IEntity<int>
    {
        public int Id { get; set; }
        public int TreatmentId { get; set; }
        public required string ImageUrl { get; set; }
        public required string Caption { get; set; }
        public int DisplayOrder { get; set; }
        public Treatment Treatment { get; set; } = default!;
    }
}
