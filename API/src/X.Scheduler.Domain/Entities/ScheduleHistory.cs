using System;

namespace X.Scheduler.Domain.Entities
{
    public class ScheduleHistory : BaseEntity
    {
        public ScheduleHistory()
        {
            this.Shift = Shift.First;
        }

        public long StaffId { get; set; }

        public DateTime Date { get; set; }

        public Shift Shift { get; set; }

        public long PreviousId { get; set; }

    }


}
