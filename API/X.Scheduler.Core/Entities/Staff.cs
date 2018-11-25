using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace X.Scheduler.Core.Entitites
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

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [JsonIgnore]
        public List<Schedule> Schedules { get; set; }

    }
}