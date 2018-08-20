using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using X.Scheduler.Data;
using X.Scheduler.Data.Entitites;


namespace X.Scheduler.Managers
{
    public sealed class ScheduleManager
    {
        private Timer Timer;
        private ApplicationContext AppContext = null;
        private DateTime FirstWorkingDay = DateTime.Today;
        private List<Schedule> ActiveSchedule = new List<Schedule>();
        private bool NeedToGenerateNewSchedule = true;

        public ScheduleManager(ApplicationContext appContext)
        {
            AppContext = appContext;
        }

        public void Initialize()
        {
            TimeSpan handleInterval = new TimeSpan(1, 0, 0);
            TimeSpan.TryParse(ConfigurationManager.AppSetting["ScheduleGeneratorInterval"], out handleInterval);
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
                //TODO: Log exception by logger
            }
            finally
            {
                Timer.Start();
            }
        }

        private void Handle()
        {
            try
            {
                FirstWorkingDay = GetFirstWorkingDayOfWeek();
                HousekeepSchedules();
                GenerateNewSchedule();
            }
            catch (Exception ex)
            {
                //TODO: Log exception by logger
            }
        }

        private void HousekeepSchedules()
        {
            if (AppContext.Schedule.Count() == 0)
            {
                NeedToGenerateNewSchedule = true;
                return;
            }

            var latestScheduleRecord = AppContext.Schedule.Max(s => s.Date);
            if (latestScheduleRecord != null && latestScheduleRecord < FirstWorkingDay)
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
            List<int> results = GetRandomNumbersListWtihAppliedRule();
            List<int> results2 = ApplyPostRule(results);

            // TODO: need to move this part to test project
            /*
            Dictionary<int, int> dic = new Dictionary<int, int>();
            foreach (var item in results2)
            {
                int count = results2.Where(x => x.Equals(item)).Count();

                if (count == 1)
                {
                    throw new Exception("Shifts caount cannot be 1. It shoud be at least 2."); // rule 3 broken
                }
                if (!dic.ContainsKey(item))
                    dic.Add(item, count);
            }
            foreach (var item in dic)
            {
                Console.WriteLine(item.Key + "," + item.Value);
            }
            */

            int currentItemIndex = 0;
            DateTime ShiftDate = FirstWorkingDay;
            foreach (var item in results2)
            {
                if (staffs[item] != null)
                {
                    Schedule sch = new Schedule();
                    sch.StaffId = staffs[item].Id;

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
                if (foundDate.DayOfWeek.ToString() == Constants.FIRST_WORKING_DAY)
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

        private List<int> GetRandomNumbersListWtihAppliedRule()
        {
            var randomNumbers = new List<int>();
            var randomizer = new Random();
            int number = 0;
            int itemsCount = Constants.SCHEDULE_DAYS * 2;
            int staffCount = AppContext.Staff.Count();

            for (int i = 0; i < itemsCount; i++)
            {
                number = ApplyPreRules(staffCount, randomNumbers, number, i);
                randomNumbers.Add(number);

            }
            return randomNumbers;
        }

        private int ApplyPreRules(int staffCount, List<int> randomNumbers, int number, int i)
        {
            if (randomNumbers.Count == 0)
            {
                return number;
            }
            else if (randomNumbers.Count >= 1)
            {
                while (i > 0 && randomNumbers[i - 1] == number)  // rule 1
                {
                    number = GetNextRandomNumber(staffCount);
                }

                while (i > 0 && i % 2 == 0 && randomNumbers[i - 2] == number) // rule 2
                {
                    number = GetNextRandomNumber(staffCount);
                }

                //foreach (var num in randomNumbers)
                //{
                int count = randomNumbers.Where(x => x.Equals(number)).Count();
                while (count == 4)
                {
                    number = GetNextRandomNumber(staffCount);
                    count = randomNumbers.Where(x => x.Equals(number)).Count();
                }
                //}

                return number;
            }

            return number;
        }

        private List<int> ApplyPostRule(List<int> shifts)
        {
            List<int> shiftsWithPostRules = new List<int>();
            List<int> empsWithOneShift = new List<int>();
            List<int> empsWithThreeShifts = new List<int>();
            int staffCount = AppContext.Staff.Count();
            shiftsWithPostRules = new List<int>(shifts);

            for (int staffNum = 0; staffNum < staffCount; staffNum++)
            {
                int count = shifts.Where(x => x.Equals(staffNum)).Count();
                int staffIndex = shifts.IndexOf(staffNum);

                if (count == 1)
                {
                    if (!empsWithOneShift.Contains(staffIndex))
                        empsWithOneShift.Add(staffIndex);
                }

                if (count >= 3)
                {
                    if (!empsWithThreeShifts.Contains(staffIndex))
                        empsWithThreeShifts.Add(staffIndex);
                }
            }

            if (empsWithOneShift.Count > 0 && empsWithThreeShifts.Count > 0)
            {
                bool hit = false;
                for (int i = 0; i < empsWithOneShift.Count; i++)
                {
                    foreach (var threeShiftedEmpIndex in empsWithThreeShifts)
                    {
                        int oneShiftedEmpNum = shiftsWithPostRules[empsWithOneShift[i]];
                        int properEmpNumAfterPreRules = ApplyPreRules(staffCount, shiftsWithPostRules, oneShiftedEmpNum, threeShiftedEmpIndex);
                        if (!oneShiftedEmpNum.Equals(properEmpNumAfterPreRules))
                        {
                            continue;
                        }
                        else
                        {
                            shiftsWithPostRules[threeShiftedEmpIndex] = oneShiftedEmpNum;
                            break;
                        }
                    }
                }
            }

            return shiftsWithPostRules;
        }


        private int GetNextRandomNumber(int maxValue)
        {
            var randomizer = new Random();
            return randomizer.Next(0, maxValue);
        }

        /// <summary>
        /// Returns all numbers, between min and max inclusive, once in a random sequence.
        /// </summary>
        IEnumerable<int> UniqueRandom(int minInclusive, int maxInclusive)
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




        public IEnumerable<int> GetSchedule(int shiftsCount, int staffsCount)
        {
            List<int> candidates = new List<int>();
            for (int i = 1; i <= shiftsCount; i++)
            {
                candidates.Add(i);
            }
            Random rnd = new Random();
            while (candidates.Count > 0)
            {
                int index = rnd.Next(staffsCount);
                //while (randomNumbers[i - 1] == number)  // rule 1
                //{
                //    number = GetNextRandomNumber(staffsCount);
                //}

                //while (i % 2 == 0 && randomNumbers[i - 2] == number) // rule 2
                //{
                //    number = GetNextRandomNumber(staffsCount);
                //}

                yield return candidates[index];
                candidates.RemoveAt(index);
            }
        }




    }
}
