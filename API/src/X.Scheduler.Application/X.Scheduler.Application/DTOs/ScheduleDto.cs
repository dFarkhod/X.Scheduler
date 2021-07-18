using System.Text.Json.Serialization;

namespace X.Scheduler.Application.DTOs
{
    public class ScheduleDto
    {

        public ScheduleDto(string staffFirstName, string staffLastName, string date, short shift, string columns)
        {
            this.staffFirstName = staffFirstName;
            this.staffLastName = staffLastName;
            Date = date;
            Shift = shift;
            Columns = columns;
        }

        [JsonIgnore]
        private string staffFirstName { get; set; }

        [JsonIgnore]
        private string staffLastName { get; set; }

        public string Date { get; set; }

        public short Shift { get; set; }

        public string Staff
        {
            get
            {
                return string.Concat(staffFirstName, " ", staffLastName);
            }
        }

        public string Columns { get; set; }
    }
}
