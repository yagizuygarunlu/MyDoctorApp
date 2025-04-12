using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Common.Results;
using WebApi.DTOs;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Appointments.Queries.GetTodays
{
    public record GetTodaysAppointmentsQuery : IRequest<Result<List<AppointmentDto>>>;

    public sealed class GetTodaysAppointmentsQueryHandler : IRequestHandler<GetTodaysAppointmentsQuery, Result<List<AppointmentDto>>>
    {
        private readonly ApplicationDbContext _context;
        public GetTodaysAppointmentsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<List<AppointmentDto>>> Handle(GetTodaysAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var appointments = await _context.Appointments
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                .AsNoTracking()
                .Where(x => x.Date.Date == today)
                .Where(x => x.Status == Domain.Enums.AppointmentStatus.Approved)
                .Select(x => new AppointmentDto(
                             x.Id,
                             x.DoctorId,
                             x.Doctor.FullName,
                             x.Patient.FullName,
                             x.Patient.Email,
                             x.Patient.PhoneNumber,
                             x.Date,
                             x.Description,
                             x.Status
                ))
                .ToListAsync(cancellationToken);
            return Result<List<AppointmentDto>>.Success(appointments);
        }
    }
}
