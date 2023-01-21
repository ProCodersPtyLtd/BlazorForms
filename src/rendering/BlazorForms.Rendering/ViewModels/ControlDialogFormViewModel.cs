using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Reflection;
using BlazorForms.Platform;
using BlazorForms.Rendering.Interfaces;
using BlazorForms.Rendering.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Shared.FastReflection;

namespace BlazorForms.Rendering
{
    public class ControlDialogFormViewModel : FormViewModel, IDialogFormViewModel
    {
        public bool DialogIsOpen { get; set; }
        public string? ItemId { get; private set; }
        public string? FlowType { get; private set; }

        public ControlDialogFormViewModel(ILogger<FormViewModel> logger, IFlowRunProvider flowRunProvider, IDynamicFieldValidator dynamicFieldValidator,
           IJsonPathNavigator jsonPathNavigator, IModelNavigator modelNavi, IReflectionProvider reflectionProvider, NavigationManager navigationManager,
           IModelBindingNavigator modelBindingNavigator)
                : base(logger, flowRunProvider, dynamicFieldValidator, jsonPathNavigator, modelNavi, reflectionProvider, navigationManager, 
                      modelBindingNavigator)
        {
        }

        public async Task LoadDialog(string flowName, FlowParamsGeneric parameters)
        {
            ClearData();

            FlowType = flowName;
            ItemId = parameters.ItemId;
            Params = parameters;

            if (string.IsNullOrWhiteSpace(flowName))
            {
                return;
            }

            FormDetails form;

            if (FormSettings.AllowFlowStorage)
            {
                var data = await _flowRunProvider.GetListItemFlowUserView(flowName, parameters);
                form = data.UserViewDetails;
                ModelUntyped = data.GetModel();
            }
            else
            {
                var startCtx = new ClientKeptContext { FlowName = flowName };
                var ctx = await _flowRunProvider.ExecuteClientKeptContextFlow(startCtx, null, parameters);

                if (ctx.ExecutionResult?.ExecutionException != null)
                {
                    throw ctx.ExecutionResult.ExecutionException;
                }

                Context = ctx.GetClientContext();
                ModelUntyped = ctx.Model;
                form = await _flowRunProvider.GetFormDetails(Context?.ExecutionResult?.FormId);
            }

            await Load(form);
            DialogIsOpen = true;
        }

        public async Task ValidateDialog()
        {
            var result = await TriggerRules(FormData.ProcessTaskTypeFullName, null, FormRuleTriggers.Submit);
            Validations = result.Validations.AsEnumerable().Union(GetDynamicFieldValidations());
        }

        public async Task SubmitDialog()
        {
            await ValidateDialog();

            if (Validations.Any(v => v.ValidationResult == RuleValidationResult.Error))
            {
                return;
            }

            if (FormSettings.AllowFlowStorage)
            {
                Context = await _flowRunProvider.SubmitListItemForm(RefId, ModelUntyped, SubmitActionName);
            }
            else
            {
                var ctx = await _flowRunProvider.SubmitClientKeptContextFlowForm(Context.GetClientContext(), ModelUntyped, 
                    Params as FlowParamsGeneric, SubmitActionName);

                Context = ctx.GetClientContext();
            }

            PopulateException(Context);
            ClearData();
            DialogIsOpen = false;
        }

        //public async Task<RuleEngineExecutionResult> TriggerDialogRules(string flowName, FlowParamsGeneric parameters)
        //{
        //    ClearData();

        //    if (string.IsNullOrWhiteSpace(flowName))
        //    {
        //        return null;
        //    }

        //    var data = await _flowRunProvider.GetListItemFlowUserView(flowName, parameters);

        //    var allFields = GetFieldsWithRules(data.UserViewDetails.Fields);
        //    var ps = Context?.Params ?? (Params as FlowParamsGeneric);
        //    var ruleRequest = GetRuleRequest(data.UserViewDetails.ProcessTaskTypeFullName, null, null, 0, allFields, ps);
        //    return await _flowRunProvider.ExecuteFormLoadRules(ruleRequest, data.GetModel());
        //}
    }
}
