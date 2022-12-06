using BlazorForms.Flows;
using BlazorForms.DynamicCode.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.DynamicCode.Validation
{
    public class FlowBaseTypeValidationRule : IDynamicCodeFlowValidationRule
    {
        public void Validate(DynamicCodeContext ctx)
        {
            var current = ctx.ClassType;

            do
            {
                if (current.IsGenericType)
                {
                    var args = current.GetGenericArguments();

                    if (args.Count() == 1)
                    {
                        var stateFlowType = typeof(StateFlowBase<>).MakeGenericType(args[0]);
                        var fluentFlowType = typeof(FluentFlowBase<>).MakeGenericType(args[0]);

                        if (current == stateFlowType || current == fluentFlowType)
                        {
                            return;
                        }
                    }
                }

                current = current.BaseType;
            }
            while (current != null);

            ctx.ValidationIssues.Add(new DynamicCodeValidationIssue 
            { 
                IsError = true, 
                Message = $"Flow type {ctx.ClassType.Name} must be inherited from StateFlowBase<> or FluentFlowBase<>" 
            });
        }
    }
}
