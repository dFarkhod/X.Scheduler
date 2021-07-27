using System.Text.Json.Serialization;

namespace X.Scheduler.Application.DTOs
{
    public record ScheduleDto
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
        private string staffFirstName { get; }

        [JsonIgnore]
        private string staffLastName { get; }

        public string Date { get; }

        public short Shift { get; }

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
