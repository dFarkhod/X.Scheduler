using System.Collections.Generic;

namespace X.Scheduler.Rules
{
    public interface IRule
    {
        string Description { get; }

        int ApplicationSequnce { get; }

        List<int> ApplyRule(List<int> inputItems, int uniqueItemsCount);


    }
}
