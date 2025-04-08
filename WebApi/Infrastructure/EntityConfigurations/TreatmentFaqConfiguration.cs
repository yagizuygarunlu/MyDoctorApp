using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Domain.Entities;

namespace WebApi.Infrastructure.EntityConfigurations
{
    public sealed class TreatmentFaqConfiguration : IEntityTypeConfiguration<TreatmentFaq>
    {
        public void Configure(EntityTypeBuilder<TreatmentFaq> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Question).IsRequired().HasMaxLength(500);
            builder.Property(t => t.Answer).IsRequired().HasMaxLength(1000);
            builder.HasOne(t => t.Treatment)
                .WithMany(t => t.Faqs)
                .HasForeignKey(t => t.TreatmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
