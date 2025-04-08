using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Domain.Entities;

namespace WebApi.Infrastructure.EntityConfigurations
{
    public sealed class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.FullName).IsRequired().HasMaxLength(100);
            builder.Property(d => d.PhoneNumber).IsRequired().HasMaxLength(15);
            builder.Property(d => d.Email).IsRequired().HasMaxLength(100);
            builder.Property(d => d.Speciality).IsRequired().HasMaxLength(50);
            builder.HasMany(d => d.Appointments)
                .WithOne(a => a.Doctor)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    } 
}
