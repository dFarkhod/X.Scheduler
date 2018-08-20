using Microsoft.EntityFrameworkCore;
using X.Scheduler.Data.Entitites;

namespace X.Scheduler.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<ScheduleHistory> ScheduleHistory { get; set; }
        public DbSet<Staff> Staff { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            new StaffMap(modelBuilder.Entity<Staff>());
            new ScheduleMap(modelBuilder.Entity<Schedule>().HasOne(x => x.Staff).WithMany(s => s.Schedules).HasForeignKey(f => f.StaffId));
            new ScheduleHistoryMap(modelBuilder.Entity<ScheduleHistory>());
        }

    }
}
