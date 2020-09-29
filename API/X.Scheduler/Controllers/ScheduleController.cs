using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using X.Scheduler.Persistence;
using X.Scheduler.Core.Entitites;
using X.Scheduler.Models;
using X.Scheduler.Core.Repositories;

namespace X.Scheduler.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [Route("api/[controller]")]
    public class ScheduleController : Controller
    {
        //private ApplicationContext AppContext = null;
        private readonly ILogger<ScheduleController> Logger;
        private readonly IAppRepository AppRepository;
        public ScheduleController(ApplicationContext appContext, ILogger<ScheduleController> logger, IAppRepository appRepository)
        {
            //AppContext = appContext;
            Logger = logger;
            AppRepository = appRepository;
        }

        // GET api/values
        [HttpGet]
        public List<ScheduleViewModel> Get()
        {
            IEnumerable<Schedule> schedule = AppRepository.GetAllSchedules(); // TODO: Instead of using Repository here, use MediatR service
            List<ScheduleViewModel> scheduleWithStaff = MapStaffToSchedule(schedule);
            Logger.LogInformation("Schedule list sent:" + JsonConvert.SerializeObject(scheduleWithStaff));
            return scheduleWithStaff;
        }

        private List<ScheduleViewModel> MapStaffToSchedule(IEnumerable<Schedule> schedule)
        {
            List<ScheduleViewModel> scheduleWithStaff = new List<ScheduleViewModel>();
            foreach (var sws in schedule)
            {
                var staff = AppRepository.GetStaffById(sws.StaffId);
                var svm = new
                    ScheduleViewModel(
                    staff.FirstName,
                    staff.LastName,
                    sws.Date.ToString("yyyy-MM-dd"),
                    (short)sws.Shift,
                    "Date,Shift,Staff");

                scheduleWithStaff.Add(svm);
            }
            scheduleWithStaff = scheduleWithStaff.OrderBy(s => s.Date).ThenBy(s => s.Shift).ToList();
            return scheduleWithStaff;
        }



        #region commented
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
        #endregion
    }
}
