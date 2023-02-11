using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Shared.Extensions;

namespace BlazorForms.Flows.Definitions
{
    public interface IFlowRunEngine
    {
        event FlowEvent OnLoad;
        event FlowEvent OnSave;

        Task<IFlowContext> ExecuteFlow(FlowRunParameters runParameters);
        Task<IFlowContext> ExecuteFlow(Type flowType, string refId, FlowParamsGeneric parameters);
        Task<IFlowContext> ExecuteFlow(string flowType, string refId, FlowParamsGeneric parameters);
        Task<IFlowContext> ContinueFlow(string refId, string operationName = null, FlowParamsGeneric flowParams = null);
        Task<UserViewModelPageResult> ExecuteFlowTask(string flowType, IFlowContext context, string userViewCallbackTaskName, QueryOptions queryOptions, dynamic dynamicParams = null);
        Task<IFlowContext> ExecuteFlowNoStorage(string flowType, FlowParamsGeneric parameters);
        Task<IFlowContext> ContinueFlowNoStorage(IFlowContext context, string operationName = null, FlowParamsGeneric flowParams = null);
        Task<IFlowContext> ContinueFlowNoStorage(ClientKeptContext context, IFlowModel model, FlowParamsGeneric flowParams);
        void UpdateOperation(string operationName);
        Task UpdateFlowContextModel(string refId, IFlowModel model);
        Task UpdateFlowContext(IFlowContext context);

        List<Type> GetAllFlowTypes();
        Task<FlowDefinitionDetails> GetFlowDefinitionDetails(FlowRunParameters runParameters);
    }
}
