using WebApi.Domain.Common;

namespace WebApi.Domain.Entities
{
    public class Review: IEntity<int>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Message { get; set; }
        public int Rating { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public bool IsApproved { get; set; }
    }
}
