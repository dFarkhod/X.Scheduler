using System.Collections.Generic;
using System.Linq;

namespace X.Scheduler.Rules
{
    public class RuleC : BaseRule, IRule
    {
        public RuleC()
        {

        }

        public override int ApplicationSequnce
        {
            get
            {
                return 3;
            }
        }

        public override string Description
        {
            get
            {
                return "Each engineer should have completed one whole day of support in any 2 week period.";
            }

        }

        public override List<int> ApplyRule(List<int> inputItems, int uniqueItemsCount)
        {
            InputItems = inputItems;
            OutputItems = new List<int>(InputItems);
            List<int> empsWithOneShift = new List<int>();
            List<int> empsWithThreeOrMoreShifts = new List<int>();

            for (int uniqueItem = 0; uniqueItem < uniqueItemsCount; uniqueItem++)
            {
                int count = OutputItems.Where(oi => oi.Equals(uniqueItem)).Count();
                int staffIndex = OutputItems.IndexOf(uniqueItem);

                if (count == 1)
                {
                    if (!empsWithOneShift.Contains(staffIndex))
                        empsWithOneShift.Add(staffIndex);
                }

                if (count >= 3)
                {
                    if (!empsWithThreeOrMoreShifts.Contains(staffIndex))
                        empsWithThreeOrMoreShifts.Add(staffIndex);
                }
            }

            if (empsWithOneShift.Count > 0 && empsWithThreeOrMoreShifts.Count > 0)
            {
                for (int i = 0; i < empsWithOneShift.Count; i++)
                {
                    foreach (var emp in empsWithThreeOrMoreShifts)
                    {
                        int oneShiftedEmpNum = OutputItems[empsWithOneShift[i]];
                        OutputItems[emp] = oneShiftedEmpNum;
                        break;

                    }
                }
            }

            return OutputItems;
        }


    }
}
