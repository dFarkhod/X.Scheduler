using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using X.Scheduler.Application;
using X.Scheduler.Application.Managers;
using X.Scheduler.Application.Services;
using X.Scheduler.Domain.Entities.Interfaces;
using X.Scheduler.Infrastructure;

namespace X.Scheduler.Shell
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private string CorsPolicyName = "CORS";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMediatR(typeof(Startup)); // because MediatR handlers are not located in the Shell project
            var assembly = AppDomain.CurrentDomain.Load("X.Scheduler.Application");
            services.AddMediatR(assembly);

            services.AddMvc();
            services.AddApplication(Configuration);
            services.AddInfrastructure(Configuration);
            
            #region CORS

            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName,
                    builder => builder
                                      .WithOrigins(Configuration.GetValue<string>("AllowedHosts").Split(";").ToArray())
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });
            #endregion

            services.AddSingleton<IRulesManager, RulesManager>();
            services.AddHostedService<ScheduleGeneratorService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors(CorsPolicyName);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            loggerFactory.AddFile("Logs/X.Scheduler_{Date}.txt");

            var applicationServices = app.ApplicationServices;
            var rulesManager = applicationServices.GetService<IRulesManager>();

            #region Initialization
            rulesManager.Initialize();
            #endregion  

        }
    }
}
