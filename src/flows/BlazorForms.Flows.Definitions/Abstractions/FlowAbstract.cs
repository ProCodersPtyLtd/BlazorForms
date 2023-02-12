using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BlazorForms.Shared.Extensions;

namespace BlazorForms.Flows.Definitions
{
    [Obsolete]
    public abstract class FlowAbstract<M> : BindingModelAbstract<M>, IFlow<M>
        where M : class, IFlowModel, new()
    {
        private ITaskExecutor _executionEngine;

        public virtual M Model { get; set; }
        public virtual FlowParamsGeneric Params { get; set; }

        public ReadOnlyCollection<TaskExecutionValidationResult> TaskExecutionValidationIssues
        {
            get
            {
                return GetCurrentContext().ExecutionResult.TaskExecutionValidationIssues.AsReadOnly();
            }
        }

        //public int FlowRunId
        //{
        //    get
        //    {
        //        return GetCurrentContext().FlowRunId;
        //    }
        //}

        public string FormLastAction
        {
            get
            {
                return GetCurrentContext().ExecutionResult.FormLastAction;
            }
        }

        public FlowSettings Settings => throw new NotImplementedException();

        public virtual void UpdateCurrentContext(string statusMessage, string assignedUser, string adminUser, string assignedTeam)
        {
            var context = GetCurrentContext();
            context.StatusMessage = statusMessage;
            context.AssignedUser = assignedUser;
            context.AdminUser = adminUser;
            context.AssignedTeam = assignedTeam;
        }

        // Task is implemented as a Class
        public virtual async Task CallTask(Type taskType)
        {
            await _executionEngine.CallTask(taskType);
        }

        public abstract Task ExecuteFlow();

        public abstract IFlowContext CreateContext();

        public virtual void SetModel(IFlowModel model)
        {
            Model = model as M;
        }

        public virtual void SetParams(FlowParamsGeneric p)
        {
            Params = p;
        }

        public virtual void SetTaskExecutor(ITaskExecutor taskExecutor)
        {
            _executionEngine = taskExecutor;
        }

        public virtual async Task UserInput(Type formType)
        {
            await _executionEngine.CallForm(formType);
        }
        public virtual async Task UserInputReview(Type formType)
        {
            await UserInput(formType);
            //throw new NotImplementedException();
        }

        public virtual async Task UserView(Type formType, Func<M> callback)
        {
            await _executionEngine.CallViewDataCallback(formType, callback.Method.Name);
        }

        public virtual async Task UserView(Type formType, Func<Task<M>> callback)
        {
            await _executionEngine.CallViewDataCallback(formType, callback.Method.Name);
        }

        public virtual async Task UserView(Type formType, Func<QueryOptions, Task<M>> callback)
        {
            await _executionEngine.CallViewDataCallback(formType, callback.Method.Name);
        }

        public virtual async Task UserView(Type formType, Func<QueryOptions, M> callback)
        {
            await _executionEngine.CallViewDataCallback(formType, callback.Method.Name);
        }

        public virtual async Task UserView(Type formType, Func<QueryOptions, dynamic, M> callback)
        {
            await _executionEngine.CallViewDataCallback(formType, callback.Method.Name);
        }

        //
        public virtual async Task FlowTask(Type formType)
        {
            await _executionEngine.CallTask(formType);
        }

        [Obsolete]
        public virtual IFlowContext GetCurrentContext()
        {
            return _executionEngine.CurrentContext;
        }

        public IFlowModel GetModel()
        {
            return Model;
        }

        public abstract Type GetModelType();
    }
}
