using BlazorForms.Rendering.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.ViewModels
{
    public class ControlViewModel
    {
        protected IFormViewModel _formViewModel;

        public virtual bool PreventCloseWithoutSave()
        {
            return false;
        }

        public virtual async Task Close() { }

        internal void RegisterParentControlViewModel(IFormViewModel formViewModel)
        {
            _formViewModel = formViewModel;
        }
    }
}
