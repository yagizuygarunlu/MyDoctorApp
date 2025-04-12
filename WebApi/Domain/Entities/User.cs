using WebApi.Domain.Common;

namespace WebApi.Domain.Entities
{
    public class User:IEntity<int>
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Admin;
    }
}
