using Microsoft.Extensions.DependencyInjection;
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
        private static readonly Lazy<ScheduleManager> lazy = new Lazy<ScheduleManager>(() => new ScheduleManager());

        public static ScheduleManager Instance { get { return lazy.Value; } }

        private Timer timer;
        private IServiceCollection services;

        public ScheduleManager()
        {

        }

        public void Initialize(IServiceCollection services)
        {
            this.services = services;
            TimeSpan handleInterval = new TimeSpan(1, 0, 0);
            TimeSpan.TryParse(ConfigurationManager.AppSetting["ScheduleGeneratorInterval"], out handleInterval);
            timer = new Timer(handleInterval.TotalMilliseconds);
            timer.Elapsed += new ElapsedEventHandler(HandleTimer_Elapsed);
            timer.Start();
            HandleTimer_Elapsed(this, null);
        }

        private void HandleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                timer.Stop();
                Handle();
            }
            catch (Exception ex)
            {
                //TODO: Log exception by logger
            }
            finally
            {
                timer.Start();
            }
        }

        private void Handle()
        {
            try
            {
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
            //TODO: Move data from Schedule to ScheduleHistory
            IRepository<ScheduleHistory> scheduleHistoryRepo = services.BuildServiceProvider().GetService<IRepository<ScheduleHistory>>();
            IRepository<Schedule> scheduleRepo = services.BuildServiceProvider().GetService<IRepository<Schedule>>();
            var existingSchedules = scheduleRepo.GetAll().ToList();
            var scheduleHistoryItems = new List<ScheduleHistory>();
            foreach (var existingSchedule in existingSchedules)
            {
                ScheduleHistory shItem = new ScheduleHistory();
                shItem.PreviousId = existingSchedule.Id;
                shItem.Shift = existingSchedule.Shift;
                shItem.StaffId = existingSchedule.StaffId;
                shItem.Date = existingSchedule.Date;
                //scheduleHistoryRepo.Insert(shItem);
                scheduleHistoryItems.Add(shItem);
            }
            scheduleHistoryRepo.InsertRange(scheduleHistoryItems);
            scheduleRepo.DeleteAll("Schedule");
        }

        public void GenerateNewSchedule()
        {
            IRepository<Staff> staffRepo = services.BuildServiceProvider().GetService<IRepository<Staff>>();
            IRepository<Schedule> scheduleRepo = services.BuildServiceProvider().GetService<IRepository<Schedule>>();

            List<Schedule> scheduleList = new List<Schedule>();

            var staffs = staffRepo.GetAllWithChildren().ToList();
            if (staffs == null || staffs.Count <= 0)
                throw new Exception("No any staff defined! Please define staff.");

            if (staffs.Count < 2)
                throw new Exception("Please define at least two staffs");


            List<int> results = GetRandomNumbersListWtihAppliedRule(Constants.SCHEDULE_DAYS * 2, staffs.Count);
            List<int> results2 = ApplyPostRule(results, staffs.Count);

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





            DateTime FirstWorkingDay = GetFirstWorkingDayOfWeek();

            // first populate first shift, then second
            int currentItemIndex = 0;
            foreach (var item in results2)
            {
                if (staffs[item] != null)
                {
                    Schedule sch = new Schedule();
                    sch.StaffId = staffs[item].Id;
                    if (scheduleList.Count < 1)
                        sch.Date = FirstWorkingDay;
                    else
                        sch.Date = FirstWorkingDay.AddDays(currentItemIndex);

                    sch.Shift = Shift.First;
                    scheduleList.Add(sch);
                }
                else
                {
                    // retrieve again from the beginning
                }
                currentItemIndex++;
            }

            // TODO: Create a single insert
            // TODO: Move existing schedule into ScheduleHistory table before insert
            foreach (var item in scheduleList)
            {
                scheduleRepo.Insert(item);
            }

            // string serializedSchedule = JsonConvert.SerializeObject(scheduleList);
            // File.WriteAllText("Data\\Schedule.json", serializedSchedule); 
        }

        // TODO: iMPLEMENT RULES AS STATED IN:
        // https://stackoverflow.com/questions/6488034/how-to-implement-a-rule-engine
        // https://mobiusstraits.com/2015/08/12/expression-trees/
        // CREATE NEW TABLES RULES in the database and retrieve them in startup


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

        private List<int> GetRandomNumbersListWtihAppliedRule(int itemsCount, int staffCount)
        {
            var randomNumbers = new List<int>();
            var randomizer = new Random();
            int number = 0;

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

        private List<int> ApplyPostRule(List<int> shifts, int staffCount)
        {
            List<int> shiftsWithPostRules = new List<int>();
            List<int> empsWithOneShift = new List<int>();
            List<int> empsWithThreeShifts = new List<int>();
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
