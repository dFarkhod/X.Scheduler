using System.Collections.Generic;

namespace X.Scheduler.Rules
{
    public interface IRule
    {
        int ApplicationSequnce { get; set; }

        List<int> ApplyRule(List<int> items);


    }
}
