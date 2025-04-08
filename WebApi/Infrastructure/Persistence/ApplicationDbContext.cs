using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Entities;
using WebApi.Domain.ValueObjects;

namespace WebApi.Infrastructure.Persistence
{
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Owned<Address>();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<PersonalLink> PersonalLinks { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<CarouselItem> CarouselItems { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<TreatmentImage> TreatmentImages { get; set; }
        public DbSet<TreatmentFaq> TreatmentFaqs { get; set; }
    }
}
