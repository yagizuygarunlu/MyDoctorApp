using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Common.Interfaces;
using WebApi.Common.Results;
using WebApi.Domain.Entities;
using WebApi.Domain.Enums;
using WebApi.DTOs;

namespace WebApi.Features.Appointments.Queries.GetAppointments
{
    public record GetAppointmentsQuery(
        int? DoctorId,
        DateTime? StartDate,
        DateTime? EndDate,
        string? PatientName,
        AppointmentStatus? AppointmentStatus) : IRequest<Result<List<AppointmentDto>>>, IParsable<GetAppointmentsQuery>
    {
        public GetAppointmentsQuery() : this(null, null, null, null, null) { }

        public static GetAppointmentsQuery Parse(string s, IFormatProvider? provider)
        {
            if (TryParse(s, provider, out var result))
                return result;

            throw new FormatException($"Could not parse query: {s}");
        }

        public static bool TryParse(string? s, IFormatProvider? provider, out GetAppointmentsQuery result)
        {
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
            var query = BuildFilteredQuery(request);

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

        private IQueryable<Appointment> BuildFilteredQuery(GetAppointmentsQuery request)
        {
            var query = _context.Appointments
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                .AsNoTracking();

            if (request.StartDate.HasValue)
                query = query.Where(x => x.Date >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                query = query.Where(x => x.Date <= request.EndDate.Value);

            if (!string.IsNullOrWhiteSpace(request.PatientName))
                query = query.Where(x => x.Patient.FullName.Contains(request.PatientName));

            if (request.AppointmentStatus.HasValue)
                query = query.Where(x => x.Status == request.AppointmentStatus.Value);

            if (request.DoctorId.HasValue)
                query = query.Where(x => x.DoctorId == request.DoctorId.Value);

            return query;
        }
    }
}
