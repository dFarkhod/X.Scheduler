using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using X.Scheduler.Data;
using X.Scheduler.Data.Entitites;
using X.Scheduler.Models;

namespace X.Scheduler.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [Route("api/[controller]")]
    public class ScheduleController : Controller
    {
        private ApplicationContext AppContext = null;
        public ScheduleController(ApplicationContext appContext)
        {
            AppContext = appContext;
        }

        // GET api/values
        [HttpGet]
        public List<ScheduleViewModel> Get()
        {
            IEnumerable<Schedule> scheduleWithStaff = AppContext.Schedule.OrderBy(x => x.Date);
            List<ScheduleViewModel> result = new List<ScheduleViewModel>();
            foreach (var sws in scheduleWithStaff)
            {
                var staff = AppContext.Staff.FirstOrDefault(s => s.Id == sws.StaffId);
                var svm = new
                    ScheduleViewModel(
                    staff.FirstName,
                    staff.LastName,
                    sws.Date.ToString("yyyy-MM-dd"),
                    (short)sws.Shift,
                    "Date,Shift,Staff");

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
