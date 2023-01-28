using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms
{
    public class BlazorFormsConfiguration
    {
        public RuleEngineType RuleEngineType { get; set; }

        public static BlazorFormsConfiguration GetDefault() 
        {
            return new BlazorFormsConfiguration() 
            {
                RuleEngineType = RuleEngineType.Simple,
            };
        }
    }

    public enum RuleEngineType
    {
        Simple,
        CascadingPull,
    }
}
