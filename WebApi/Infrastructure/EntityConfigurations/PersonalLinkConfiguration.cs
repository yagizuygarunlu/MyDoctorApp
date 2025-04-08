using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Domain.Entities;

namespace WebApi.Infrastructure.EntityConfigurations
{
    public sealed class PersonalLinkConfiguration : IEntityTypeConfiguration<PersonalLink>
    {
        public void Configure(EntityTypeBuilder<PersonalLink> builder)
        {
            builder.HasKey(pl => pl.Id);
            builder.Property(pl => pl.Url).IsRequired().HasMaxLength(200);
            builder.HasOne(pl => pl.Doctor)
                .WithMany(d => d.PersonalLinks)
                .HasForeignKey(pl => pl.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
