using Microsoft.EntityFrameworkCore;
using X.Scheduler.Data;
using X.Scheduler.Data.Entitites;

namespace X.Scheduler.Repository
{
    public class ScheduleRepository
    {
        ApplicationContext ApplicationContext = null;
        public ScheduleRepository(ApplicationContext applicationContext)
        {
            ApplicationContext = applicationContext;
        }

        public DbSet<Schedule> GetSchedule()
        {
            return ApplicationContext.Schedule;
        }

    }
}
