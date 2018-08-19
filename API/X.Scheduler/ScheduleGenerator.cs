using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using X.Scheduler.Data.Entitites;

//TODO: Create a poller which runs every 1 hour by the timer and checks whether need to generate a new schedule by checking table and first day of the week

namespace X.Scheduler
{
    public sealed class ScheduleGenerator
    {
        private static readonly Lazy<ScheduleGenerator> lazy = new Lazy<ScheduleGenerator>(() => new ScheduleGenerator());

        public static ScheduleGenerator Instance { get { return lazy.Value; } }

        private List<Staff> staffs = new List<Staff>();

        public ScheduleGenerator()
        {
            staffs = new List<Staff>();
        }


        public void GenerateSchedule(IServiceCollection services)
        {
            IRepository<Staff> staffRepo = services.BuildServiceProvider().GetService<IRepository<Staff>>();
            IRepository<Schedule> scheduleRepo = services.BuildServiceProvider().GetService<IRepository<Schedule>>();

            List<Schedule> scheduleList = new List<Schedule>();

            staffs = staffRepo.GetAllWithChildren().ToList();
            if (staffs == null || staffs.Count <= 0)
                throw new Exception("No any staff defined! Please define staff.");

            if (staffs.Count < 2)
                throw new Exception("Please define at least two staffs");


            List<int> results = GetRandomNumbersListWtihAppliedRule(Constants.SCHEDULE_DAYS * 2, staffs.Count);
            List<int> results2 = ApplyPostRule(results, staffs.Count);


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

        private List<Staff> GetAllStaff()
        {
            List<Staff> staffList = new List<Staff>();
            string fileContent = File.ReadAllText("Data\\staff.json");
            staffList = JsonConvert.DeserializeObject<List<Staff>>(fileContent);
            return staffList;
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
