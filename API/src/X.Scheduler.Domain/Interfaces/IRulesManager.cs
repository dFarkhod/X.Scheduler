using System.Collections.Generic;

namespace X.Scheduler.Domain.Entities.Interfaces
{
    public interface IRulesManager
    {
        void Initialize();
        void Initialize(string pathToRulesLib);
        List<int> ApplyRules(List<int> inputItems, int uniqueItemsCount);
    }
}