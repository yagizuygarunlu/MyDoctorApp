using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Results;
using WebApi.Domain.Enums;
using WebApi.DTOs;
using WebApi.Infrastructure.Persistence;

namespace WebApi.Features.Appointments.Queries.GetAppointments
{
    public record GetAppointmentsQuery(
        int? DoctorId,
        DateTime? StartDate,
        DateTime? EndDate,
        string? PatientName,
        AppointmentStatus? AppointmentStatus) : IRequest<Result<List<AppointmentDto>>>, IParsable<GetAppointmentsQuery>
    {
        // Default constructor needed for binding
        public GetAppointmentsQuery() : this(null, null, null, null, null) { }

        // Implementation of IParsable interface
        public static GetAppointmentsQuery Parse(string s, IFormatProvider? provider)
        {
            if (TryParse(s, provider, out var result))
                return result;

            throw new FormatException($"Could not parse query: {s}");
        }

        public static bool TryParse(string? s, IFormatProvider? provider, out GetAppointmentsQuery result)
        {
            // This method won't actually be called with the full query string
            // It's just required by the interface
            result = new GetAppointmentsQuery();
            return true;
        }
    }

    public sealed class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, Result<List<AppointmentDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetAppointmentsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<List<AppointmentDto>>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Appointments
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                .AsNoTracking()
                .AsQueryable();
            if (request.StartDate.HasValue)
            {
                query = query.Where(x => x.Date >= request.StartDate.Value);
            }
            if (request.EndDate.HasValue)
            {
                query = query.Where(x => x.Date <= request.EndDate.Value);
            }
            if (!string.IsNullOrWhiteSpace(request.PatientName))
            {
                query = query.Where(x => x.Patient.FullName.Contains(request.PatientName));
            }
            if (request.AppointmentStatus.HasValue)
            {
                query = query.Where(x => x.Status == request.AppointmentStatus.Value);
            }
            if (request.DoctorId.HasValue)
            {
                query = query.Where(x => x.DoctorId == request.DoctorId.Value);
            }
            var appointments = await query
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
