﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using X.Scheduler.Application.DTOs;
using X.Scheduler.Domain.Entities;
using X.Scheduler.Domain.Entities.Interfaces;

namespace X.Scheduler.Application.Queries
{
    public class GetScheduleListQuery : IRequest<List<ScheduleDto>>
    {
    }

    public class GetScheduleListQueryHandler : IRequestHandler<GetScheduleListQuery, List<ScheduleDto>>
    {
        private readonly IAppRepository _appRepository;

        public GetScheduleListQueryHandler(IAppRepository appRepository)
        {
            _appRepository = appRepository;
        }
        public Task<List<ScheduleDto>> Handle(GetScheduleListQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Schedule> schedule = _appRepository.GetAllSchedules();
            var result = MapStaffToSchedule(schedule);
            return Task.FromResult(result);
        }

        private List<ScheduleDto> MapStaffToSchedule(IEnumerable<Schedule> schedule)
        {
            List<ScheduleDto> scheduleWithStaff = new List<ScheduleDto>();
            foreach (var sws in schedule)
            {
                var staff = _appRepository.GetStaffById(sws.StaffId);
                var svm = new
                    ScheduleDto(
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

    }
}
