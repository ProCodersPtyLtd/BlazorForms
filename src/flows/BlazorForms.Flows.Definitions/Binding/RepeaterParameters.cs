using BlazorForms.Shared.BindingModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class RepeaterParameters : BindingParameters
    {
        public bool IsFixedList { get; set; }
        public int? CurrentRow { get; set; }
        public bool IsLineEditing { get; set; }
    }
}
