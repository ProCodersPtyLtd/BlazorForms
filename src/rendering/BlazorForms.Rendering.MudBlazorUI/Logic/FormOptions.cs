using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.MudBlazorUI
{
    public abstract class FormOptions : FormOptionsBase
    {
        public Variant Variant { get; set; }
        public bool MudBlazorProvidersDefined { get; set; }
    }
}
    