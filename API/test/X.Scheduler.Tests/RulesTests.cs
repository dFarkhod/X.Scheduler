using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using X.Scheduler.Application.Managers;

namespace X.Scheduler.Tests
{
    [TestClass]
    public class RulesTests
    {

        [TestMethod]
        public void AnyEmployeeWithOneShift()
        {
            RulesManager rm = new RulesManager();
            rm.Initialize(@"C:\Projects\X.Scheduler\API\X.Scheduler\bin\Debug\netcoreapp3.1\");
            List<int> inputList = new List<int>();
            Random rnd = new Random();
            for (int i = 0; i < 28; i++)
            {
                inputList.Add(rnd.Next(0, 9));
            }
            List<int> resultList = new List<int>();
            resultList = rm.ApplyRules(inputList, 9);

            foreach (var item in resultList)
            {
                int count = resultList.Where(x => x.Equals(item)).Count();

                if (count.Equals(1))
                    Assert.Fail();
            }
        }

        [TestMethod]
        public void AnyEmployeeWithTwoDaysSecondShift()
        {
            RulesManager rm = new RulesManager();
            rm.Initialize(@"C:\Projects\X.Scheduler\API\X.Scheduler\bin\Debug\netcoreapp3.1\");
            List<int> inputList = new List<int>();
            Random rnd = new Random();
            for (int i = 0; i < 28; i++)
            {
                inputList.Add(rnd.Next(0, 9));
            }
            List<int> resultList = new List<int>();
            resultList = rm.ApplyRules(inputList, 9);

            int index = 0;
            foreach (var item in resultList)
            {
                while (index > 0 && index % 2 == 0 && item.Equals(resultList[index - 2]))
                {
                    Assert.Fail();
                }
                index++;
            }
        }
    }
}
