using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using X.Scheduler.Data;
using X.Scheduler.Data.Entitites;


namespace X.Scheduler.Managers
{
    public class ScheduleManager : BaseManager
    {
        private Timer Timer;
        private ApplicationContext AppContext = null;
        private RulesManager RulesManager = null;
        private DateTime FirstWorkingDayDate = DateTime.Today;
        private List<Schedule> ActiveSchedule = new List<Schedule>();
        private bool NeedToGenerateNewSchedule = true;
        private int SchedulePeriodInDays = 14;
        private string FirstWorkingWeekDay = "Monday";

        public ScheduleManager(ApplicationContext appContext, RulesManager rm)
        {
            AppContext = appContext;
            RulesManager = rm;
        }

        public override void Initialize()
        {
            TimeSpan handleInterval = new TimeSpan(1, 0, 0);
            TimeSpan.TryParse(ConfigurationManager.AppSetting["ScheduleGeneratorInterval"], out handleInterval);
            int.TryParse(ConfigurationManager.AppSetting["SchedulePeriodInDays"], out SchedulePeriodInDays);
            FirstWorkingWeekDay = ConfigurationManager.AppSetting["FirstWorkingWeekDay"];
            Timer = new Timer(handleInterval.TotalMilliseconds);
            Timer.Elapsed += new ElapsedEventHandler(HandleTimer_Elapsed);
            Timer.Start();
            HandleTimer_Elapsed(this, null);
        }

        private void HandleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Timer.Stop();
                Handle();
            }
            catch (Exception ex)
            {
                //TODO: Implement logging
            }
            finally
            {
                Timer.Start();
            }
        }

        private void Handle()
        {
            FirstWorkingDayDate = GetFirstWorkingDayOfWeek();
            HousekeepSchedules();
            GenerateNewSchedule();
        }

        private void HousekeepSchedules()
        {
            if (AppContext.Schedule.Count() == 0)
            {
                NeedToGenerateNewSchedule = true;
                return;
            }

            var latestScheduleRecord = AppContext.Schedule.Max(s => s.Date);
            if (latestScheduleRecord != null && latestScheduleRecord < FirstWorkingDayDate)
            {
                var scheduleHistoryItems = new List<ScheduleHistory>();
                var existingSchedules = AppContext.Schedule;
                foreach (var existingSchedule in existingSchedules)
                {
                    ScheduleHistory shItem = new ScheduleHistory();
                    shItem.PreviousId = existingSchedule.Id;
                    shItem.Shift = existingSchedule.Shift;
                    shItem.StaffId = existingSchedule.StaffId;
                    shItem.Date = existingSchedule.Date;
                    scheduleHistoryItems.Add(shItem);
                }
                AppContext.ScheduleHistory.AddRange(scheduleHistoryItems);

                var truncateScheduleTable = "TRUNCATE TABLE [Schedule];";
                AppContext.Database.ExecuteSqlCommand(truncateScheduleTable);
                AppContext.SaveChanges();
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

            List<Staff> staffs = AppContext.Staff.ToList();
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

                AppContext.Schedule.AddRange(ActiveSchedule);
                AppContext.SaveChanges();
            }
            catch (Exception ex)
            {
                //TODO: Implement logging
            }
        }

        private bool ValidateStaffsCount()
        {
            if (!AppContext.Staff.Any())
            {
                throw new Exception("No any staff defined! Please define staff.");
            }

            if (AppContext.Staff.Count() < 2)
                throw new Exception("Please define at least two staffs");

            return true;
        }

        private DateTime GetFirstWorkingDayOfWeek()
        {
            DateTime foundDate = DateTime.Today;
            for (int dayOfWeek = 1; dayOfWeek <= 7; dayOfWeek++)
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
            return foundDate;

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


    }
}
