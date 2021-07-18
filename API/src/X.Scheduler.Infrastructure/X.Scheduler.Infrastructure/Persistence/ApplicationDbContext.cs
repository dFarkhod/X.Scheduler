using Microsoft.EntityFrameworkCore;
using X.Scheduler.Domain.Entities;
using X.Scheduler.Infrastructure.Persistence.Maps;

namespace X.Scheduler.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<ScheduleHistory> ScheduleHistory { get; set; }
        public DbSet<Staff> Staff { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
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
