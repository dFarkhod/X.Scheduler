using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using X.Scheduler.Domain.Entities.Interfaces;
using X.Scheduler.Infrastructure.Persistence;
using X.Scheduler.Domain.Entities;
using X.Scheduler.Application.DTOs;
using System.Threading.Tasks;
using X.Scheduler.Application.Queries;

namespace X.Scheduler.Shell.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [Route("api/[controller]")]
    public class ScheduleController : ApiControllerBase
    {

        //todo: remove appDbContext here and also reference to the infrastructure from shell and replace it with MediatR
        private readonly ILogger<ScheduleController> Logger;
        public ScheduleController(ApplicationDbContext appContext, ILogger<ScheduleController> logger, IAppRepository appRepository)
        {
            Logger = logger;
        }

        [HttpGet]
        public async Task<List<ScheduleDto>> GetAsync()
        {
            List<ScheduleDto> scheduleWithStaff = await Mediator.Send(new GetScheduleListQuery());
            Logger.LogInformation("Schedule list sent:" + JsonConvert.SerializeObject(scheduleWithStaff));
            return scheduleWithStaff;
        }

    }
}
