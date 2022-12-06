using BlazorForms.FlowRules;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.Interfaces
{
    public interface IDialogFormViewModel : IFormViewModel
    {
        bool DialogIsOpen { get; set; }
        string? ItemId { get;  }
        string? FlowType { get; }

        Task LoadDialog(string flowType, FlowParamsGeneric parameters);
        //Task<RuleEngineExecutionResult> TriggerDialogRules(string flowType, FlowParamsGeneric parameters);
        Task ValidateDialog();
        Task SubmitDialog();
    }
}
