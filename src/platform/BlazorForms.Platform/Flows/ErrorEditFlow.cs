using Microsoft.Extensions.Logging;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Platform.ProcessFlow;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Flows;
using BlazorForms.FlowRules;
using System.Linq;
using BlazorForms.Shared.Extensions;
using BlazorForms.Platform.Shared.Attributes;
using BlazorForms.Platform.Definitions.Model;

namespace BlazorForms.Platform
{
    [Flow(nameof(ErrorEditFlow))]
    [Form("Error Description")]
    public class ErrorEditFlow : FluentFlowBase<ErrorModel>
    {
        private readonly ILogger _logger;

        public ErrorEditFlow(ILogger<ErrorEditFlow> logger)
        {
            _logger = logger;
        }

        public override void Define()
        {
            this
                .Begin()
                .Next(() => PopulateDataAsync())
                .NextForm(typeof(ErrorEditFlowForm))
                .End();
        }
        
        public virtual async Task PopulateDataAsync()
        {
            Model.Created = DateTime.UtcNow;
            Model.Message = Params.GetParam("Message");
            Model.Type = Params.GetParam("Type");
            Model.StackTrace = Params.GetParam("StackTrace");
        }
    }

    [Form("Error Description")]
    public class ErrorEditFlowForm : FlowTaskDefinitionBase<ErrorModel>
    {
        [FormComponent(typeof(DateEdit))]
        [Display("Date", Disabled = true)]
        public object DateControl => ModelProp(m => m.Created);

        [FormComponent(typeof(TextEdit))]
        [Display("Type", Disabled = true)]
        public object TypeControl => ModelProp(m => m.Type);

        [FormComponent(typeof(TextArea))]
        [Display("Message", Disabled = true)]
        public object MessageControl => ModelProp(m => m.Message);

        [FormComponent(typeof(TextArea))]
        [Display("Stack Trace", Disabled = true)]
        public object StackTraceControl => ModelProp(m => m.StackTrace);

        [Display("Close")]
        public object CloseButton => ActionButton(ActionType.Close);
    }

    
}
