using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using X.Scheduler.Domain;
using X.Scheduler.Domain.Entities;
using X.Scheduler.Domain.Entities.Interfaces;

namespace X.Scheduler.Application.Services
{
    public class ScheduleGeneratorService : BackgroundService
    {
        private IRulesManager _rulesManager = null;
        private IConfiguration _configuration;
        private IAppRepository _scheduleRepo;
        private ILogger<ScheduleGeneratorService> _logger;
        private IServiceScopeFactory _serviceScopeFactory;
        private IServiceScope _scope;

        private DateTime FirstWorkingDayDate = DateTime.Today;
        private List<Schedule> ActiveSchedule = new List<Schedule>();
        private bool NeedToGenerateNewSchedule = true;
        private int SchedulePeriodInDays = 14;
        private string FirstWorkingWeekDay = "Monday";

        public ScheduleGeneratorService(IRulesManager rm, IConfiguration configuration, ILogger<ScheduleGeneratorService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _rulesManager = rm;
            _configuration = configuration;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            Initialize();
        }

        private void Initialize()
        {
            FirstWorkingWeekDay = _configuration["FirstWorkingWeekDay"];
        }

        // todo: Convert this method to async
        private void HandleJob()
        {
            using (_scope = _serviceScopeFactory.CreateScope())
            {
                GetFirstWorkingDayOfWeek();
                HousekeepSchedules();
                GenerateNewSchedule();
            }
        }

        private void HousekeepSchedules()
        {
            _scheduleRepo = _scope.ServiceProvider.GetRequiredService<IAppRepository>();
            if (_scheduleRepo.GetScheduleCount() == 0)
            {
                NeedToGenerateNewSchedule = true;
                return;
            }

            var latestScheduleRecord = _scheduleRepo.GetLatestScheduleRecord();
            if (latestScheduleRecord < FirstWorkingDayDate)
            {
                var scheduleHistoryItems = new List<ScheduleHistory>();
                var existingSchedules = _scheduleRepo.GetAllSchedules();
                foreach (var existingSchedule in existingSchedules)
                {
                    ScheduleHistory shItem = new ScheduleHistory();
                    shItem.PreviousId = existingSchedule.Id;
                    shItem.Shift = existingSchedule.Shift;
                    shItem.StaffId = existingSchedule.StaffId;
                    shItem.Date = existingSchedule.Date;
                    scheduleHistoryItems.Add(shItem);
                }
                _scheduleRepo.AddToHistory(scheduleHistoryItems);
                _scheduleRepo.DeleteAllSchedules();
                NeedToGenerateNewSchedule = true;
            }
            else
            {
                NeedToGenerateNewSchedule = false;
            }
        }

        public void GenerateNewSchedule()
        {
            if (!NeedToGenerateNewSchedule)
                return;

            ValidateStaffsCount();

            List<Staff> staffs = _scheduleRepo.GetAllStaff();
            int StaffsCount = staffs.Count();
            int scheduleItemsCount = SchedulePeriodInDays * 2;
            List<int> uniqueNumberList = GetUniqueNumbersList(StaffsCount, scheduleItemsCount);

            List<int> staffIndexListWithRules = _rulesManager.ApplyRules(uniqueNumberList, StaffsCount - 1);
            GenerateSchedule(staffs, staffIndexListWithRules);
        }

        private List<int> GetUniqueNumbersList(int StaffsCount, int scheduleItemsCount)
        {
            List<int> uniqueNumberList = new List<int>(scheduleItemsCount);
            while (uniqueNumberList.Count < scheduleItemsCount)
            {
                IEnumerable<int> newItems = GetUniqueRandomNumbers(0, StaffsCount - 1);
                uniqueNumberList.AddRange(newItems);
            }

            if (uniqueNumberList.Count > scheduleItemsCount)
            {
                int countToRemove = uniqueNumberList.Count - scheduleItemsCount;
                uniqueNumberList.RemoveRange(scheduleItemsCount, countToRemove);
            }

            return uniqueNumberList;
        }

        private void GenerateSchedule(List<Staff> staffs, List<int> scheduleListWithRules)
        {
            int currentItemIndex = 0;
            try
            {
                DateTime ShiftDate = FirstWorkingDayDate;
                foreach (var item in scheduleListWithRules)
                {
                    if (staffs[item] != null)
                    {
                        Schedule sch = new Schedule();
                        sch.StaffId = staffs[item].Id; //item

                        if (currentItemIndex == 0 || currentItemIndex % 2 == 0)
                        {
                            sch.Shift = Shift.First;
                        }
                        else
                        {
                            sch.Shift = Shift.Second;
                        }

                        sch.Date = ShiftDate;
                        ActiveSchedule.Add(sch);
                        if (currentItemIndex > 0 && currentItemIndex % 2 != 0)
                            ShiftDate = ShiftDate.AddDays(1);
                    }
                    currentItemIndex++;
                }

                _scheduleRepo.AddRange(ActiveSchedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured while Generating Schedule");
                throw;
            }
        }

        private bool ValidateStaffsCount()
        {
            if (_scheduleRepo.GetStaffCount() <= 0)
            {
                throw new Exception("No any staff defined! Please define staff.");
            }

            if (_scheduleRepo.GetStaffCount() < 2)
                throw new Exception("Please define at least two staffs");

            return true;
        }

        private void GetFirstWorkingDayOfWeek()
        {
            DateTime foundDate = DateTime.Today;
            for (int dayOfWeek = 1; dayOfWeek <= Constants.DAYS_IN_WEEK; dayOfWeek++)
            {
                if (foundDate.DayOfWeek.ToString().Equals(FirstWorkingWeekDay))
                {
                    break;
                }
                else
                {
                    foundDate = DateTime.Today.AddDays(dayOfWeek);
                }
            }
            FirstWorkingDayDate = foundDate;
        }

        private IEnumerable<int> GetUniqueRandomNumbers(int minInclusive, int maxInclusive)
        {
            List<int> candidates = new List<int>();
            for (int i = minInclusive; i <= maxInclusive; i++)
            {
                candidates.Add(i);
            }
            Random rnd = new Random();
            while (candidates.Count > 0)
            {
                int index = rnd.Next(candidates.Count);
                yield return candidates[index];
                candidates.RemoveAt(index);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            HandleJob();
            return Task.CompletedTask;
        }
    }
}
