using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Domain.Entities;

namespace WebApi.Infrastructure.EntityConfigurations
{
    public sealed class TreatmentImageConfiguration : IEntityTypeConfiguration<TreatmentImage>
    {
        public void Configure(EntityTypeBuilder<TreatmentImage> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.ImageUrl).IsRequired().HasMaxLength(500);
            builder.Property(t => t.Caption).IsRequired().HasMaxLength(200);
            builder.Property(t => t.DisplayOrder).IsRequired();
            builder.HasOne(t => t.Treatment)
                .WithMany(t => t.Images)
                .HasForeignKey(t => t.TreatmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
