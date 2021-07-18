using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using Quartz;
using X.Scheduler.Domain.Entities.Interfaces;
using X.Scheduler.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using X.Scheduler.Domain;

namespace X.Scheduler.Application.Managers
{
    [DisallowConcurrentExecution]
    public class ScheduleGeneratorJob : BaseManager, IScheduleManager, IJob
    {
        private IRulesManager RulesManager = null;
        private DateTime FirstWorkingDayDate = DateTime.Today;
        private List<Schedule> ActiveSchedule = new List<Schedule>();
        private bool NeedToGenerateNewSchedule = true;
        private int SchedulePeriodInDays = 14;
        private string FirstWorkingWeekDay = "Monday";
        private IConfiguration Configuration;
        private IAppRepository ScheduleRepo;
        private ILogger<ScheduleGeneratorJob> Logger;

        public ScheduleGeneratorJob(IRulesManager rm, IAppRepository repo, IConfiguration configuration, ILogger<ScheduleGeneratorJob> logger)
        {
            ScheduleRepo = repo;
            RulesManager = rm;
            Configuration = configuration;
            Logger = logger;
            Initialize();
        }

        public override void Initialize()
        {
            FirstWorkingWeekDay = Configuration["FirstWorkingWeekDay"];
        }

        // todo: Convert this method to async
        private void HandleJob()
        {
            GetFirstWorkingDayOfWeek();
            HousekeepSchedules();
            GenerateNewSchedule();
        }

        private void HousekeepSchedules()
        {
            if (ScheduleRepo.GetScheduleCount() == 0)
            {
                NeedToGenerateNewSchedule = true;
                return;
            }

            var latestScheduleRecord = ScheduleRepo.GetLatestScheduleRecord();
            if (latestScheduleRecord != null && latestScheduleRecord < FirstWorkingDayDate)
            {
                var scheduleHistoryItems = new List<ScheduleHistory>();
                var existingSchedules = ScheduleRepo.GetAllSchedules();
                foreach (var existingSchedule in existingSchedules)
                {
                    ScheduleHistory shItem = new ScheduleHistory();
                    shItem.PreviousId = existingSchedule.Id;
                    shItem.Shift = existingSchedule.Shift;
                    shItem.StaffId = existingSchedule.StaffId;
                    shItem.Date = existingSchedule.Date;
                    scheduleHistoryItems.Add(shItem);
                }
                ScheduleRepo.AddToHistory(scheduleHistoryItems);
                ScheduleRepo.DeleteAllSchedules();
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

            List<Staff> staffs = ScheduleRepo.GetAllStaff();
            int StaffsCount = staffs.Count();
            int scheduleItemsCount = SchedulePeriodInDays * 2;
            List<int> uniqueNumberList = GetUniqueNumbersList(StaffsCount, scheduleItemsCount);

            List<int> staffIndexListWithRules = RulesManager.ApplyRules(uniqueNumberList, StaffsCount - 1);
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

                ScheduleRepo.AddRange(ActiveSchedule);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception occured while Generating Schedule");
                throw;
            }
        }

        private bool ValidateStaffsCount()
        {
            if (ScheduleRepo.GetStaffCount() <= 0)
            {
                throw new Exception("No any staff defined! Please define staff.");
            }

            if (ScheduleRepo.GetStaffCount() < 2)
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

        public Task Execute(IJobExecutionContext context)
        {
            HandleJob();
            return Task.CompletedTask;
        }
    }
}
