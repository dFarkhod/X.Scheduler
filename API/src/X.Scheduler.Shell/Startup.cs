using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System;
using MediatR;
using X.Scheduler.Infrastructure.Persistence;
using X.Scheduler.Domain.Entities.Interfaces;
using X.Scheduler.Infrastructure.Persistence.Repositories;
using X.Scheduler.Application.Managers;

namespace X.Scheduler.Shell
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup));
            services.AddMvc();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnString")));

            #region CORS
           
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });
            #endregion


            services.AddSingleton<IRulesManager, RulesManager>();
            services.AddScoped<IAppRepository, AppRepository>();

            services.AddQuartz(q =>
            {
                // base quartz scheduler, job and trigger configuration
                // handy when part of cluster or you want to otherwise identify multiple schedulers
                q.SchedulerId = "Scheduler-Core";

                // we take this from appsettings.json, just show it's possible
                // q.SchedulerName = "Quartz ASP.NET Core Sample Scheduler";

                // we could leave DI configuration intact and then jobs need to have public no-arg constructor
                // the MS DI is expected to produce transient job instances 
              /*  q.UseMicrosoftDependencyInjectionJobFactory(options =>
                {
                    // if we don't have the job in DI, allow fallback to configure via default constructor
                    options.AllowDefaultConstructor = true;
                });*/

                // or 
                 q.UseMicrosoftDependencyInjectionScopedJobFactory();

                // these are the defaults
                q.UseSimpleTypeLoader();

                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });

                TimeSpan handleInterval = new TimeSpan(1, 0, 0);
                TimeSpan.TryParse(Configuration["ScheduleGeneratorInterval"], out handleInterval);

                // configure jobs with code
                var jobKey = new JobKey("Schedule Generator Job", "Schedule Generator Job Group");
                q.AddJob<ScheduleGeneratorJob>(j => j
                    .StoreDurably()
                    .WithIdentity(jobKey)
                    .WithDescription("Schedule Generator Job")
                );

                q.AddTrigger(t => t
                    .WithIdentity("Schedule Generator Job Trigger")
                    .ForJob(jobKey)
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithInterval(handleInterval).RepeatForever())
                    .WithDescription("Schedule Generator Job Trigger")
                );

                // convert time zones using converter that can handle Windows/Linux differences
                q.UseTimeZoneConverter();
                q.UseInMemoryStore();

                /*q.UsePersistentStore(s =>
                {
                    s.UseProperties = true;
                    s.RetryInterval = TimeSpan.FromSeconds(15);
                    s.UseSqlServer(sqlServer =>
                    {
                        sqlServer.ConnectionString = Configuration.GetConnectionString("DefaultConnString");
                        sqlServer.TablePrefix = "QRTZ_";
                    });

                    s.UseJsonSerializer();
                    s.UseClustering(c =>
                    {
                        c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                        c.CheckinInterval = TimeSpan.FromSeconds(10);
                    });
                });*/

            });

                // ASP.NET Core hosting
                services.AddQuartzServer(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });


            //services.AddSingleton<IScheduleManager, ScheduleManager>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            loggerFactory.AddFile("Logs/X.Scheduler_{Date}.txt");
            //app.UseCors("SiteCorsPolicy");

            var applicationServices = app.ApplicationServices;
            var rulesManager = applicationServices.GetService<IRulesManager>();
            //var scheduleManager = applicationServices.GetService<IScheduleManager>();
           // app.ApplicationServices.GetService<IScheduleManager>();
            #region Initialization
            rulesManager.Initialize();
            //scheduleManager.Initialize();
            #endregion  

        }
    }
}
