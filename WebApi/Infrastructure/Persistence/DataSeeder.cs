using WebApi.Domain.Entities;
using WebApi.Infrastructure.Services;

namespace WebApi.Infrastructure.Persistence
{
    public static class DataSeeder
    {
        public static void SeedAdminUser(ApplicationDbContext context, PasswordService passwordService)
        {
            if (!context.Users.Any())
            {
                var adminUser = new User
                {
                    FullName = "Admin User",
                    Email = "admin@mydoctorapp.com",
                    PasswordHash = passwordService.HashPassword("Admin123!"), // İlk şifre
                    Role = UserRole.Admin
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }
        }
    }
}
