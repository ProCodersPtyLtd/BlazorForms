using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BlazorForms.Shared
{
    public enum FormRuleTriggers
    {
        [Description("Submit")]
        Submit = 1,
        [Description("Loaded")]
        Loaded,
        [Description("Changed")]
        Changed,
        [Description("ItemAdded")]
        ItemAdded,
        [Description("ItemChanged")]
        ItemChanged,
        [Description("ContextMenuClicking")]
        ContextMenuClicking,
    }
}
