using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Forms
{
    public enum ButtonActionTypes
    {
        // new Forms
        Custom = 1,
        Cancel,
        Close,
        Submit, //approve
        Save,
        Validate,
        Ignore,
        Reject,
        CloseFinish,
        SubmitClose,

        // new SQLForms buttons
        Add,
        Delete
    }
}
