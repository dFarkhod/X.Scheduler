
namespace X.Scheduler.Models
{
    public class ScheduleViewModel
    {

        public ScheduleViewModel(string staffFirstName, string staffLastName, string date, short shift)
        {
            StaffFirstName = staffFirstName;
            StaffLastName = staffLastName;
            Date = date;
            Shift = shift;
        }

        public string StaffFirstName { get; set; }

        public string StaffLastName { get; set; }

        public string Date { get; set; }

        public short Shift { get; set; }

        public string StaffFullName
        {
            get
            {
                return string.Concat(StaffFirstName, " ", StaffLastName);
            }
        }
    }
}
