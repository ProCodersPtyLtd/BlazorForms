using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public enum TaskExecutionFlowStateEnum
    {
        [Description("Stop")]
        Stop = 0,
        [Description("In progress")]
        Continue,
        [Description("Finished")]
        Finished
    }
}
