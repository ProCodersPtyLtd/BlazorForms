using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.DynamicCode.Interfaces
{
    public interface IDynamicCodeValidationRule
    {
        void Validate(DynamicCodeContext ctx);
    }

    public interface IDynamicCodeFlowValidationRule : IDynamicCodeValidationRule
    {
    }
}
