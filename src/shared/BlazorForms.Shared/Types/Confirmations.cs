using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Shared
{
    public enum ConfirmType
    {
        ChangesWillBeLost,
        Delete,
        DeleteItem,
    }

    public enum ConfirmButtons
    {
        OkCancel,
        YesNo,
    }
}
