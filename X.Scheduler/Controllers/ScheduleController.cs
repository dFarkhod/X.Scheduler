using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using X.Scheduler.Data.Entitites;
using X.Scheduler.Models;

namespace X.Scheduler.Controllers
{
    [Route("api/[controller]")]
    public class ScheduleController : Controller
    {
        private IRepository<Schedule> scheduleRepo;
        private IRepository<Staff> staffRepo;

        public ScheduleController(IRepository<Schedule> schedules, IRepository<Staff> staffs)
        {
            scheduleRepo = schedules;
            staffRepo = staffs;
        }


        // GET api/values
        [HttpGet]
        public List<ScheduleViewModel> Get()
        {
            IEnumerable<Schedule> scheduleWithStaff = scheduleRepo.GetAllWithChildren(s => s.Staff);
            List<ScheduleViewModel> result = new List<ScheduleViewModel>();
            foreach (var sws in scheduleWithStaff)
            {
                var svm = new
                    ScheduleViewModel(
                    sws.Staff.FirstName,
                    sws.Staff.LastName,
                    sws.Date.ToString("yyyy-MM-dd"),
                    (short)sws.Shift);

                result.Add(svm);
            }
            return result;
        }

        // GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
