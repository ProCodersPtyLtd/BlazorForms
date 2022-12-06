using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform
{
    public interface IFlowRunProvider
    {
        // Forms
        Task SaveForm(string refId, IFlowModel model, string actionBinding = null, string operationName = null);
        Task FinishFlow(string refId, IFlowModel model, string actionBinding = null, string operationName = null);
        Task<IFlowContext> SubmitForm(string refId, IFlowModel model, string actionBinding = null, string operationName = null, FlowParamsGeneric flowParams = null);
        Task<IFlowContext> RejectForm(string refId, IFlowModel model, string actionBinding = null, string operationName = null);
        Task<IUserViewModel> GetFlowDefaultReadonlyView(string refId);
        Task<FlowRunUserViewDetails> GetCurrentFlowRunUserView(string refId, IFlowContext flowContext = null);
        //Task<IUserViewModel> GetUserViewModelPage(string refId, string userViewCallbackTaskName, int index, int pageSize);
        Task<IFlowContext> GetFlowRunExecutionContext(string refId);
        Task<FormDetails> GetFormDetails(string formName);
        Task<IFlowModel> GetLastModel(string refId);

        // Rules
        Task<RuleEngineExecutionResult> ExecuteFormLoadRules(RuleExecutionRequest ruleRequest, IFlowModel model);
        Task<RuleEngineExecutionResult> TriggerRule(RuleExecutionRequest ruleRequest, IFlowModel model);
        Task<RuleEngineExecutionResult> TriggerRule(string formTypeFullName, IFlowModel model, FlowParamsGeneric flowParams, FieldDisplayDetails[] displayProperties, FormRuleTriggers? trigger,
            string ruleCode, FieldBinding fieldBinding, int rowIndex = 0);

        IAsyncEnumerable<string> GetActiveFlowsIds(string flowName);
        IAsyncEnumerable<string> GetAllWaitingFlowsIds();
        IEnumerable<string> GetRegisteredFlows();
        Task<IFlowContext> ExecuteFlow(string flowType, string refId, FlowParamsGeneric parameters, string operationName = null, bool noStorage = false);
        Task WarmUp();

        Task<IUserViewModel> GetListFlowUserView(string flowType, FlowParamsGeneric parameters, QueryOptions queryOptions);
        Task<IUserViewModel> GetListItemFlowUserView(string flowType, FlowParamsGeneric parameters);
        Task<IFlowContext> SubmitListItemForm(string refId, IFlowModel model, string operationName = null);
        Task<bool> CheckFormAccess(FormAccessDetails access, IFlowContext context);
        Task<bool> CheckFormAccess(FormAccessDetails access, UserViewAccessInformation accessDetails, IFlowModel model, FlowParamsGeneric parameters);

        // silent flow (no storage)
        [Obsolete("use ExecuteClientKeptContextFlow")]
        Task<IUserViewModel> ExecuteSilentFlowForm(string flowType, FlowParamsGeneric flowParams);
        [Obsolete("use SubmitClientKeptContextFlowForm")]
        Task<IFlowContext> SubmitSilentFlowForm(string flowType, IFlowModel model, string actionBinding = null, string operationName = null, FlowParamsGeneric flowParams = null);
        // No storage and client keeps context
        Task<IFlowContext> ExecuteClientKeptContextFlow(ClientKeptContext ctx, IFlowModel model, FlowParamsGeneric parameters);
        Task<IFlowContext> SubmitClientKeptContextFlowForm(ClientKeptContext ctx, IFlowModel model, FlowParamsGeneric parameters, string actionBinding);

        // Validations
        void ValidateFlow(Type flowType);

    }
}
