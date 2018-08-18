using System;

namespace X.Scheduler.Data.Entitites
{
    public class Schedule : BaseEntity
    {
        public Schedule()
        {
            this.Shift = Shift.First;
        }

        public long StaffId { get; set; }

        public Staff Staff { get; set; }

        public DateTime Date { get; set; }

        public Shift Shift { get; set; }

    }

    public enum Shift : short
    {
        First = 1,
        Second = 2
    }
}
