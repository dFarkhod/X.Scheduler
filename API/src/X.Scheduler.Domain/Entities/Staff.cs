using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace X.Scheduler.Domain.Entities
{
    public class Staff : BaseEntity
    {
        public Staff()
        {

        }

        public Staff(long id, string firstName, string lastName, string email, string title)
        {
            base.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Title = title;

        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Title { get; set; }

        [JsonIgnore]
        public List<Schedule> Schedules { get; set; }

    }
}