using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Entities;

namespace WebApi.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Doctor> Doctors { get; }
        DbSet<Appointment> Appointments { get; }
        DbSet<Review> Reviews { get; }
        DbSet<Patient> Patients { get; }
        DbSet<CarouselItem> CarouselItems { get; }
        DbSet<Platform> Platforms { get; }
        DbSet<Treatment> Treatments { get; }
        DbSet<TreatmentFaq> TreatmentFaqs { get; }
        DbSet<TreatmentImage> TreatmentImages { get; }
        DbSet<PersonalLink> PersonalLinks { get; }
        DbSet<User> Users { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
