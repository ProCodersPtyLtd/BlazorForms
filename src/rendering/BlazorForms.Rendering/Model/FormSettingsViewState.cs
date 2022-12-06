using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering
{
    public class FormSettingsViewState
    {
        public bool AllowAnonymousAccess  { get; set; }
        public bool AllowFlowStorage { get; set; }
        public bool IsDefaultReadonlyView { get; set; }
        public string NavigationPathSuccess { get; set; }
        public string NavigationPathCancel { get; set; }
    }
}
