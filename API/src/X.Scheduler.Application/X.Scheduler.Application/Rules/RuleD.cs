using System.Collections.Generic;

namespace X.Scheduler.Application.Rules
{
    public class RuleD : BaseRule, IRule
    {
        public RuleD()
        {

        }

        public override int ApplicationSequnce
        {
            get
            {
                return 4;
            }
        }

        public override string Description
        {
            get
            {
                return "If an engineer work on two consecutive days are eligible to get two days exemption.";
            }

        }

        public override List<int> ApplyRule(List<int> inputItems, int uniqueItemsCount)
        {
            InputItems = inputItems;
            OutputItems = new List<int>(InputItems);
            // THIS RULE DOESN'T NEED TO BE IMPLEMENTED AS IT CONTRADICTS WITH RULES #1 AND #2

            return OutputItems;
        }


    }
}
