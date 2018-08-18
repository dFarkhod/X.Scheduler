using Microsoft.EntityFrameworkCore;
using X.Scheduler.Data;
using X.Scheduler.Data.Entitites;

namespace X.Scheduler
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            new StaffMap(modelBuilder.Entity<Staff>());
            new ScheduleMap(modelBuilder.Entity<Schedule>().HasOne(x => x.Staff).WithMany(s => s.Schedules).HasForeignKey(f => f.StaffId));

        }

    }
}
