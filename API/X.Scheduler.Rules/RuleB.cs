using System.Collections.Generic;

namespace X.Scheduler.Rules
{
    public class RuleB : BaseRule, IRule
    {
        public RuleB()
        {

        }

        public override int ApplicationSequnce
        {
            get
            {
                return 2;
            }
        }

        public override string Description
        {
            get
            {
                return "An engineer cannot have two afternoon shifts on consecutive days.";
            }

        }

        public override List<int> ApplyRule(List<int> inputItems, int uniqueItemsCount)
        {
            InputItems = inputItems;
            OutputItems = new List<int>(InputItems);
            for (int i = 0; i < OutputItems.Count; i++)
            {
                CurrentIndex = i;
                while (i > 0 && i % 2 == 0 && CurrentItem.Equals(OutputItems[CurrentIndex - 2]))
                {
                    int number = NextItem;
                    if (number.Equals(NEGATIVE))
                        number = GetNextRandomNumber(OutputItems.Count);

                    OutputItems[CurrentIndex] = number;
                }
            }

            return OutputItems;
        }

    }
}
