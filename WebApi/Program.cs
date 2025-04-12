using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.CarouselItems;
using WebApi.Features.Doctors;
using WebApi.Features.Reviews;
using WebApi.Features.TreatmentFaqs;
using WebApi.Features.Treatments;
using WebApi.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
var app = builder.Build();
app.MapCarouselItemEndpoints();
app.MapDoctorEndpoints();
app.MapReviewEndpoints();
app.MapTreatmentEndpoints();
app.MapTreatmentFaqEndpoints();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
