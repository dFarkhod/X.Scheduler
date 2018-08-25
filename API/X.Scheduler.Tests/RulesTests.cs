using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace X.Scheduler.Tests
{
    [TestClass]
    public class ScheduleFactoryTests
    {
        [TestMethod]
        public void TestRandomNumbers()
        {
            // ScheduleGenerator.Instance.GenerateSchedule(Constants.SCHEDULE_DAYS * 2, 10);
            //ScheduleGenerator sf = new ScheduleGenerator();
            //List<int> randomList = sf.GetRandomNumbersList(Constants.SCHEDULE_DAYS * 2, 10);
            //Assert.IsNotNull(randomList);
        }
        [TestMethod]
        public void TestScheduleGenerator()
        {
            //ScheduleGenerator.Instance.GenerateNewSchedule();

            //ScheduleGenerator sf = new ScheduleGenerator();
            //List<int> randomList = sf.GetRandomNumbersList(Constants.SCHEDULE_DAYS * 2, 10);
            //Assert.IsNotNull(randomList);



            // TODO: need to move this part to test project
            /*
            
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
        }
    }
}
