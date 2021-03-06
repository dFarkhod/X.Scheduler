﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using X.Scheduler.Core.Abstracts;
using X.Scheduler.Rules;

namespace X.Scheduler.Managers
{
    public class RulesManager : BaseManager, IRulesManager
    {
        public RulesManager()
        {

        }

        private List<IRule> Rules = new List<IRule>();

        public override void Initialize()
        {
            LoadRules();
        }

        private void LoadRules(string pathToRulesLib = "")
        {
            if (string.IsNullOrEmpty(pathToRulesLib))
            {
                pathToRulesLib = Assembly.GetEntryAssembly().Location;
                pathToRulesLib = Path.GetDirectoryName(pathToRulesLib);
            }
            string rulesLibrary = Path.Combine(pathToRulesLib, typeof(IRule).Namespace + ".dll");
            Assembly rulesAssembly = Assembly.LoadFrom(rulesLibrary);
            List<Type> ruleTypes = GetTypesByInterface<IRule>(rulesAssembly);
            ruleTypes.ForEach(rt =>
            {
                if (!rt.Attributes.ToString().Contains("Abstract"))
                {
                    IRule rule = Activator.CreateInstance(rt) as IRule;
                    Rules.Add(rule);
                }
            });

            Rules = Rules.OrderBy(o => o.ApplicationSequnce).ToList();
        }

        public void Initialize(string pathToRulesLib)
        {
            LoadRules(pathToRulesLib);
        }

        public List<int> ApplyRules(List<int> inputItems, int uniqueItemsCount)
        {
            List<int> outputItems = new List<int>(inputItems);
            Rules.ForEach(rule =>
            {
                outputItems = ApplyRule(outputItems, rule, uniqueItemsCount);
            });
            return outputItems;
        }

        private List<int> ApplyRule(List<int> inputItems, IRule rule, int uniqueItemsCount)
        {
            List<int> outputItems = new List<int>(inputItems);
            outputItems = rule.ApplyRule(outputItems, uniqueItemsCount);
            return outputItems;
        }

        private List<Type> GetTypesByInterface<T>(Assembly assembly)
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("T must be an interface");

            return assembly.GetTypes()
                .Where(x => x.GetInterface(typeof(T).Name) != null)
                .ToList<Type>();
        }
    }
}
