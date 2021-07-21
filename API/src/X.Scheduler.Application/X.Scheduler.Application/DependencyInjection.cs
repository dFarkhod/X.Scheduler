using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using X.Scheduler.Application.Managers;
using X.Scheduler.Application.Services;
using X.Scheduler.Domain.Entities.Interfaces;

namespace X.Scheduler.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IRulesManager, RulesManager>();
            services.AddHostedService<ScheduleGeneratorService>();
            return services;
        }
    }
}