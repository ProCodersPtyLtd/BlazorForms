using BlazorForms.DynamicCode.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.DynamicCode.Engine
{
    public class DynamicCodeValidationEngine : IDynamicCodeValidationEngine
    {
        public DynamicCodeValidationEngine()
        { }

        public void Validate(Type ruleInterface, DynamicCodeContext ctx)
        {
            var result = new List<DynamicCodeValidationIssue>();
            var rules = this.GetType().Assembly.GetTypes().Where(t => ruleInterface.IsAssignableFrom(t) && t.IsClass);

            foreach (var ruleType in rules)
            {
                var rule = Activator.CreateInstance(ruleType) as IDynamicCodeValidationRule;
                rule.Validate(ctx);
            }

            
        }
    }
}
