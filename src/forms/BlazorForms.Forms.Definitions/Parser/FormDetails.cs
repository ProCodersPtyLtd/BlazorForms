using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BlazorForms.Forms
{
    public class FormDetails : ContainerDetails
    {
        public string ProcessTaskTypeFullName { get; set; }
        public string DisplayName { get; set; }
        public string ChildProcessTypeFullName { get; set; }
        public string PkColumn { get; set; }
        public FormAccessDetails Access { get; set; }
    }
}
