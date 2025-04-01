using WebApi.Domain.Common;

namespace WebApi.Domain.Entities
{
    public class Platform : IEntity<int>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string IconClass { get; set; }
        public string? ColorHex { get; set; }
    }
}
