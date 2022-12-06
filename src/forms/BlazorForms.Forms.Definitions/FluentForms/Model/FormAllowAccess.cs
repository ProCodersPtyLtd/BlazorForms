using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Forms
{
    public class FormAllowAccess
    {
        public bool Allow { get; set; }
        public bool OnlyAssignee { get; set; }
        public string Roles { get; set; }
        public Type CustomRule { get; set; }
    }
}
