using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.DynamicCode
{
    public interface IDynamicFlowProvider
    {
        DynamicCodeContext GetFlow(string fullName);
        void RemoveFlow(string fullName);
        DynamicCodeContext CompileFlow(DynamicCodeParameters ps);
    }
}
