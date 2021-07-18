using System.Collections.Generic;

namespace X.Scheduler.Rules
{
    public class RuleA : BaseRule, IRule
    {
        public RuleA()
        {

        }

        public override int ApplicationSequnce
        {
            get
            {
                return 1;
            }
        }

        public override string Description
        {
            get
            {
                return "An engineer can do at most one-half day shift in a day.";
            }

        }

        public override List<int> ApplyRule(List<int> inputItems, int uniqueItemsCount)
        {
            InputItems = inputItems;
            OutputItems = new List<int>(InputItems);
            for (int i = 0; i < OutputItems.Count; i++)
            {
                CurrentIndex = i;
                while (i > 0 && CurrentItem.Equals(PreviousItem))
                {
                    int number = NextItem;
                    if (number.Equals(NEGATIVE))
                        number = GetNextRandomNumber(uniqueItemsCount);

                    OutputItems[i] = number;
                }
            }

            return OutputItems;
        }

    }
}
