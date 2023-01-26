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
using BlazorForms.Rendering.Model;

namespace BlazorForms.Rendering
{
    public class BoardDialogViewModel : FormViewModel
    {
        public string? FormType { get; private set; }

        public BoardDialogViewModel(ILogger<BoardDialogViewModel> logger, IFlowRunProvider flowRunProvider, IDynamicFieldValidator dynamicFieldValidator,
           IJsonPathNavigator jsonPathNavigator, IModelNavigator modelNavi, IReflectionProvider reflectionProvider, NavigationManager navigationManager,
           IModelBindingNavigator modelBindingNavigator)
                : base(logger, flowRunProvider, dynamicFieldValidator, jsonPathNavigator, modelNavi, reflectionProvider, navigationManager, 
                      modelBindingNavigator)
        {
        }

        public async Task LoadDialog(string formTypeName, CardInfo<IFlowBoardCard> card, FlowParamsGeneric? parameters = null)
        {
            ClearData();

            FormType = formTypeName;
            ModelUntyped = card.Item;
            Params = parameters;

            if (string.IsNullOrWhiteSpace(formTypeName))
            {
                return;
            }

            var form = await _flowRunProvider.GetFormDetails(formTypeName);

            await Load(form);
        }

        public async Task ValidateDialog()
        {
            var result = await TriggerRules(FormData.ProcessTaskTypeFullName, null, FormRuleTriggers.Submit);
            Validations = result.Validations.AsEnumerable().Union(GetDynamicFieldValidations());
        }

        public async Task<bool> SubmitDialog()
        {
            await ValidateDialog();

            if (Validations.Any(v => v.ValidationResult == RuleValidationResult.Error))
            {
                return false;
            }

            await Close();

            //if (FormSettings.AllowFlowStorage)
            //{
            //    Context = await _flowRunProvider.SubmitListItemForm(RefId, ModelUntyped, SubmitActionName);
            //}
            //else
            //{
            //    var ctx = await _flowRunProvider.SubmitClientKeptContextFlowForm(Context.GetClientContext(), ModelUntyped, 
            //        Params as FlowParamsGeneric, SubmitActionName);

            //    Context = ctx.GetClientContext();
            //}

            //PopulateException(Context);
            ClearData();
            return true;
        }
    }
}
