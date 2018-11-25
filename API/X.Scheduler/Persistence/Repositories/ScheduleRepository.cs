using System.Collections.Generic;
using X.Scheduler.Persistence;
using X.Scheduler.Core.Entitites;
using X.Scheduler.Core.Repositories;

namespace X.Scheduler.Persistence.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        ApplicationContext ApplicationContext = null;
        public ScheduleRepository(ApplicationContext applicationContext)
        {
            ApplicationContext = applicationContext;
        }

        public IEnumerable<Schedule> GetSchedule()
        {
            return ApplicationContext.Schedule;
        }

    }
}
