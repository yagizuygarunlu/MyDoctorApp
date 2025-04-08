using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Domain.Entities;

namespace WebApi.Infrastructure.EntityConfigurations
{
    public class CarouselItemConfiguration : IEntityTypeConfiguration<CarouselItem>
    {
        public void Configure(EntityTypeBuilder<CarouselItem> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.Property(ci => ci.ImageUrl).HasMaxLength(150).IsRequired();
            builder.Property(ci => ci.Title).IsRequired().HasMaxLength(100);
            builder.Property(ci => ci.Description).HasMaxLength(500);
        }
    }
}
