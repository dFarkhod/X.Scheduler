using System.Collections.Generic;
using X.Scheduler.Persistence;
using X.Scheduler.Core.Entitites;
using X.Scheduler.Core.Repositories;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System;
using Microsoft.EntityFrameworkCore;

namespace X.Scheduler.Persistence.Repositories
{
    public class AppRepository : IAppRepository
    {
        private readonly ApplicationContext ApplicationContext = null;
        public AppRepository(ApplicationContext applicationContext)
        {
            ApplicationContext = applicationContext;
        }

        public long GetScheduleCount()
        {
            return ApplicationContext.Schedule.Count();
        }

        public DateTime GetLatestScheduleRecord()
        {
            return ApplicationContext.Schedule.Max(s => s.Date);
        }

        public IList<Schedule> GetAllSchedules()
        {
            return ApplicationContext.Schedule.ToList();
        }

        public long GetStaffCount()
        {
            return ApplicationContext.Staff.Count();
        }

        public List<Staff> GetAllStaff()
        {
            return ApplicationContext.Staff.ToList();
        }

        public Staff GetStaffById(long staffId)
        {
            return ApplicationContext.Staff.FirstOrDefault(s => s.Id.Equals(staffId));
        }

        public void AddRange(IList<Schedule> schedules)
        {
            ApplicationContext.Schedule.AddRange(schedules);
            ApplicationContext.SaveChanges();
        }

        public void AddToHistory(IList<ScheduleHistory> history)
        {
            ApplicationContext.ScheduleHistory.AddRange(history);
            ApplicationContext.SaveChanges();
        }

        public void DeleteAllSchedules()
        {
            var truncateScheduleTable = "TRUNCATE TABLE [Schedule];";
            ApplicationContext.Database.ExecuteSqlRaw(truncateScheduleTable);
            ApplicationContext.SaveChanges();
        }

    }
}
