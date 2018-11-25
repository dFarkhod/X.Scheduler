using System.Collections.Generic;

namespace X.Scheduler.Core.Abstracts
{
    public interface IRulesManager
    {
        List<int> ApplyRules(List<int> inputItems, int uniqueItemsCount);
        void Initialize();
        void Initialize(string pathToRulesLib);
    }
}