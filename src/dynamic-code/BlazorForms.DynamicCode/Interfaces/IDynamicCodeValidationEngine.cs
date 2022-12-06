using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.DynamicCode.Interfaces
{
    public interface IDynamicCodeValidationEngine
    {
        public void Validate(Type ruleInterface, DynamicCodeContext ctx);
    }
}
