using BlazorForms.Forms.Definitions.FluentForms.Model;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.Model
{
    public class FormConfirmationData : FormConfirmationDetails
    {
        public string CancelName { get; private set; }
        public string OkName { get; private set; }

        public FormConfirmationData(FormConfirmationDetails details)
        {
            details.ReflectionCopyTo(this);

            switch (details.Buttons)
            {
                case Shared.ConfirmButtons.YesNo:
                    CancelName = "No";
                    OkName= "Yes";
                    break;
                case Shared.ConfirmButtons.OkCancel:
                default:
                    CancelName = "Cancel";
                    OkName= "OK";
                    break;
            }
        }
    }
}
