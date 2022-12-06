using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public enum FormTaskStateEnum
    {
        Initialized = 0,
        Loaded,
        Saved,
        Submitted,
        Rejected
    }
}
