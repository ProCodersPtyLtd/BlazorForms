using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Definitions
{
    public interface ITaskExecutor
    {
        IFlowContext CurrentContext { get; }

        Task CallTask(Type taskType);
        Task CallForm(Type formType);
        Task CallViewDataCallback(Type formType, string callbackMethodName);
    }
}
