using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using X.Scheduler.Data;
using X.Scheduler.Managers;

namespace X.Scheduler
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
            services.AddMvc();
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultCS")));

            #region CORS
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin(); // For anyone access.
            corsBuilder.AllowCredentials();

            services.AddCors(options =>
            {
                options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
            });
            #endregion


            //ConfigurationManager cm = new ConfigurationManager();
            services.AddSingleton<IConfigurationManager, ConfigurationManager>();
            services.AddSingleton<IRulesManager, RulesManager>();
            services.AddSingleton<IScheduleManager, ScheduleManager>();

            var serviceProvider = services.BuildServiceProvider();
            var configManager = serviceProvider.GetService<IConfigurationManager>();
            var rulesManager = serviceProvider.GetService<IRulesManager>();
            var scheduleManager = serviceProvider.GetService<IScheduleManager>();


            #region Initialization
            configManager.Initialize();
            rulesManager.Initialize();
            scheduleManager.Initialize();
            #endregion         


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            loggerFactory.AddFile("Logs/X.Scheduler_{Date}.txt");
            app.UseMvc();
            app.UseCors("SiteCorsPolicy");

        }
    }
}
