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

        private ScheduleGenerator()
        {
        }


        //TODO: PASS Repository by injection then trigger it during startup
        public void GenerateSchedule()
        {
            List<Schedule> result = new List<Schedule>();

            List<Staff> staffs = GetAllStaff(); //TODO: Get staffs from database, not from file!!
            if (staffs == null || staffs.Count <= 0)
                throw new Exception("No any staff defined! Please define staff.");

            if (staffs.Count < 2)
                throw new Exception("Please define at least two staffs");

            List<int> randomNumbers = GetRandomNumbersList(Constants.SCHEDULE_DAYS * 2, staffs.Count); // because we have to handle shifts as well
            DateTime FirstWorkingDay = GetFirstWorkingDayOfWeek();

            // first populate first shift, then second
            int currentItemIndex = 0;
            foreach (var item in randomNumbers)
            {

                if (staffs[item] != null)
                {
                    Schedule sch = new Schedule();
                    sch.StaffId = staffs[item].Id;
                    if (result.Count < 1)
                        sch.Date = FirstWorkingDay;
                    else
                        sch.Date = FirstWorkingDay.AddDays(currentItemIndex);

                    sch.Shift = Shift.First;
                    result.Add(sch);
                }
                else
                {
                    // retrieve again from the beginning
                }
                currentItemIndex++;
            }

            string serializedSchedule = JsonConvert.SerializeObject(result);
            File.WriteAllText("Data\\Schedule.json", serializedSchedule);
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

        public List<int> GetRandomNumbersList(int itemsCount, int staffsCount)
        {
            var randomNumbers = new List<int>();
            var randomizer = new Random();
            int number = 0;

            for (int i = 0; i < itemsCount; i++)
            {
                number = GetNextRandomNumber(staffsCount);
                if (randomNumbers.Count == 0)
                {
                    randomNumbers.Add(number);
                }
                else if (randomNumbers.Count >= 1)
                {
                    while (randomNumbers[i - 1] == number)  // rule 1
                    {
                        number = GetNextRandomNumber(staffsCount);
                    }

                    while (i % 2 == 0 && randomNumbers[i - 2] == number) // rule 2
                    {
                        number = GetNextRandomNumber(staffsCount);
                    }
                    randomNumbers.Add(number);
                }

            }

            // rule 3 - if somehow some staff is working more than 1 whole day, and there is another staff who only works for half day, then replace them
            int halfSize = randomNumbers.Count / 2;
            IEnumerable<IEnumerable<int>> splittedNumbers = ListHelper.Split<int>(randomNumbers, halfSize);
            foreach (IEnumerable<int> numbers in splittedNumbers)
            {

                foreach (int num in numbers)
                {
                    int count = numbers.Where(x => x.Equals(num)).Count();
                    if (count == 1)
                    {
                        //TODO: need to add one more shift
                    }
                    if (count == 3)
                    {
                        //TODO: replace with the one whose count is 1
                    }
                }
            }

            return randomNumbers;
        }

        private int GetNextRandomNumber(int maxValue)
        {
            var randomizer = new Random();
            return randomizer.Next(0, maxValue);
        }
    }
}
