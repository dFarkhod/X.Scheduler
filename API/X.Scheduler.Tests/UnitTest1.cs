using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

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
        public void TestGetSchedule()
        {
            List<int> result = ScheduleGenerator.Instance.GetSchedule(Constants.SCHEDULE_DAYS * 2, 10).ToList();
            //ScheduleGenerator sf = new ScheduleGenerator();
            //List<int> randomList = sf.GetRandomNumbersList(Constants.SCHEDULE_DAYS * 2, 10);
            //Assert.IsNotNull(randomList);
        }
    }
}
