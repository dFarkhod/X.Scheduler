using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System;
using Microsoft.EntityFrameworkCore;
using X.Scheduler.Domain.Entities.Interfaces;
using X.Scheduler.Domain.Entities;

namespace X.Scheduler.Infrastructure.Persistence.Repositories
{
    public class AppRepository : IAppRepository
    {
        private readonly ApplicationDbContext _appDbContext = null;
        public AppRepository(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public long GetScheduleCount()
        {
            return _appDbContext.Schedule.Count();
        }

        public DateTime GetLatestScheduleRecord()
        {
            return _appDbContext.Schedule.Max(s => s.Date);
        }

        public IList<Schedule> GetAllSchedules()
        {
            return _appDbContext.Schedule.ToList();
        }

        public long GetStaffCount()
        {
            return _appDbContext.Staff.Count();
        }

        public List<Staff> GetAllStaff()
        {
            return _appDbContext.Staff.ToList();
        }

        public Staff GetStaffById(long staffId)
        {
            return _appDbContext.Staff.FirstOrDefault(s => s.Id.Equals(staffId));
        }

        public void AddRange(IList<Schedule> schedules)
        {
            _appDbContext.Schedule.AddRange(schedules);
            _appDbContext.SaveChanges();
        }

        public void AddToHistory(IList<ScheduleHistory> history)
        {
            _appDbContext.ScheduleHistory.AddRange(history);
            _appDbContext.SaveChanges();
        }

        public void DeleteAllSchedules()
        {
            var truncateScheduleTable = "TRUNCATE TABLE [Schedule];";
            _appDbContext.Database.ExecuteSqlRaw(truncateScheduleTable);
            _appDbContext.SaveChanges();
        }

    }
}
