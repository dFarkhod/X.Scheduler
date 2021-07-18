using System;
using System.Collections.Generic;

namespace X.Scheduler.Rules
{
    public abstract class BaseRule : IRule, IDisposable
    {
        public abstract int ApplicationSequnce { get; }
        public abstract string Description { get; }
        public abstract List<int> ApplyRule(List<int> inputItems, int uniqueItemsCount);

        protected List<int> InputItems { get; set; }
        protected List<int> OutputItems { get; set; }

        protected const int NEGATIVE = -1;
        protected int CurrentIndex;
        protected int PreviousItem
        {
            get
            {
                if (CurrentIndex > 0)
                {
                    return OutputItems[CurrentIndex - 1];
                }
                else
                {
                    return NEGATIVE;
                }
            }

        }
        protected int NextItem
        {
            get
            {
                if (OutputItems.Count <= CurrentIndex + 1)
                {
                    return OutputItems[CurrentIndex + 1];
                }
                else
                {
                    return NEGATIVE;
                }
            }
        }
        protected int CurrentItem
        {
            get
            {
                return OutputItems[CurrentIndex];
            }
        }

        public BaseRule()
        {
            InputItems = new List<int>();
            OutputItems = new List<int>();
        }

        protected virtual int GetNextRandomNumber(int maxValue)
        {
            var randomizer = new Random();
            return randomizer.Next(0, maxValue);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                InputItems = null;
                OutputItems = null;
            }
        }
    }
}
