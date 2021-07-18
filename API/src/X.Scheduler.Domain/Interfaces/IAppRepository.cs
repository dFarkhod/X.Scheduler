using System;
using System.Collections.Generic;

namespace X.Scheduler.Domain.Entities.Interfaces
{
    public interface IAppRepository
    {
        long GetScheduleCount();
        
        DateTime GetLatestScheduleRecord();

        IList<Schedule> GetAllSchedules();

        long GetStaffCount();

        List<Staff> GetAllStaff();

        Staff GetStaffById(long staffId);

        void AddRange(IList<Schedule> schedules);

        void AddToHistory(IList<ScheduleHistory> history);

        void DeleteAllSchedules();

    }
}