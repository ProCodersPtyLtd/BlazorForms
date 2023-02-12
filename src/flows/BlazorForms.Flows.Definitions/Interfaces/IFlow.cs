using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Shared.Extensions;

namespace BlazorForms.Flows.Definitions
{
    public interface IFlow
    {
        IFlowModel GetModel();
        Type GetModelType();
        void SetModel(IFlowModel model);
        void SetParams(FlowParamsGeneric p);
        //Task ExecuteFlow();
        //Task CallTask(Type taskType);
        //Task UserInput(Type formType);
        //Task UserInputReview(Type formType);
        //void SetTaskExecutor(ITaskExecutor taskExecutor);
        IFlowContext CreateContext();
        IFlowContext GetCurrentContext();
        FlowSettings Settings { get; }

        //ReadOnlyCollection<TaskExecutionValidationResult> TaskExecutionValidationIssues { get; }
    }

    public interface IFlow<M> : IFlow where M : IFlowModel
    {
        M Model { get; set; }
        FlowParamsGeneric Params { get; set; }
        Task UserView(Type formType, Func<M> callback);
        Task UserView(Type formType, Func<Task<M>> callback);
        Task UserView(Type formType, Func<QueryOptions, M> callback);
        Task FlowTask(Type formType);
    }
}
