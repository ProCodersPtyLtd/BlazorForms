using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Forms.Definitions.FluentForms.Model
{
    public class FormConfirmationDetails
    {
        public FieldBinding Binding { get; set; }
        public ConfirmType Type { get; set; }
        public string Message { get; set; }
        public ConfirmButtons Buttons { get; set; }
    }
}
