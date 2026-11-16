using Hrms.Application.Interfaces;
using Hrms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hrms.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(connectionString));
        services.AddScoped(typeof(IHrmsRepository<>), typeof(EfRepository<>));
        services.AddScoped<IVacancyService, Hrms.Application.Services.VacancyService>();
        services.AddScoped<ICandidateService, Hrms.Application.Services.CandidateService>();
        services.AddScoped<IResumeService, Hrms.Application.Services.ResumeService>();
        services.AddScoped<IInterviewService, Hrms.Application.Services.InterviewService>();
        services.AddScoped<IReportService, Hrms.Application.Services.ReportService>();
        return services;
    }
}
