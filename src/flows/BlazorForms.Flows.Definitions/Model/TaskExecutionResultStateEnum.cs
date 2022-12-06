using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public enum TaskExecutionResultStateEnum
    {
        [Description("Failed")]
        Fail = 0,
        [Description("Success")]
        Success
    }
}
