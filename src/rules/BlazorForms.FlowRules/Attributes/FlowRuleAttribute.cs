using BlazorForms.FlowRules;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.FlowRules
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FlowRuleAttribute : Attribute
    {
        public Type RuleType { get; private set; }
        public string Name { get; private set; }
        public string Namespace { get; private set; }
        public string FullName { get; private set; }
        public FormRuleTriggers Trigger { get; private set; }

        public FlowRuleAttribute(Type ruleType, FormRuleTriggers trigger = FormRuleTriggers.Submit)
        {
            Trigger = trigger;
            RuleType = ruleType;
            Name = ruleType.Name;
            Namespace = ruleType.Namespace;
            FullName = ruleType.FullName;
        }
    }
}
