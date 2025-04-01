using WebApi.Domain.Common;

namespace WebApi.Domain.Entities
{
    public class TreatmentFaq: IEntity<int>
    {
        public int Id { get; set; }
        public int TreatmentId { get; set; }
        public required string Question { get; set; }
        public required string Answer { get; set; }
        public Treatment Treatment { get; set; } = default!;
    }
}
