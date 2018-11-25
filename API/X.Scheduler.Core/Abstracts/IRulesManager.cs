using System.Collections.Generic;

namespace X.Scheduler.Core.Abstracts
{
    public interface IRulesManager
    {
        void Initialize();
        void Initialize(string pathToRulesLib);
        List<int> ApplyRules(List<int> inputItems, int uniqueItemsCount);
    }
}