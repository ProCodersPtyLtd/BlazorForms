using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.ViewModels
{
    public class ControlViewModel
    {
        public virtual bool PreventCloseWithoutSave()
        {
            return false;
        }
    }
}
