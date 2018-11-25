using System.Collections.Generic;
using X.Scheduler.Core.Entitites;

namespace X.Scheduler.Core.Repositories
{
    public interface IScheduleRepository
    {
        IEnumerable<Schedule> GetSchedule(); 
    }
}