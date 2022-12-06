using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.Model
{
    public class FormField
    {
        public string ModelBinding { get; set; }
        public string ItemsBinding { get; set; }
        public string IdBinding { get; set; }
        public string NameBinding { get; set; }

        public string ControlType { get; set; }
        public string Name { get; set; }
        public string Caption { get; set; }
    }
}
