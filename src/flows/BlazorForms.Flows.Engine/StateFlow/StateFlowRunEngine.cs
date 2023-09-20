using Microsoft.Extensions.Logging;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Engine.StateFlow
{
    public class StateFlowRunEngine : IStateFlowRunEngine
    {
        public const int MAX_LOOP_COUNT = 1000;

        public event FlowEvent OnLoad;
        public event FlowEvent OnSave;

        protected readonly IFlowRunStorage _storage;
        protected readonly ILogger<StateFlowRunEngine> _logger;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IFlowParser _parser;

        public StateFlowRunEngine(ILogger<StateFlowRunEngine> logger, IServiceProvider serviceProvider, IFlowRunStorage storage, IFlowParser parser)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _storage = storage;
            _parser = parser;
        }

        public async Task<IFlowContext> ContinueFlow(string refId, string operationName = null, FlowParamsGeneric flowParams = null)
        {
            return await ContinueFlow(refId, null, operationName, flowParams);
        }

        public async Task<IFlowContext> ContinueFlow(string refId, IFlowModel model, string operationName = null, FlowParamsGeneric flowParams = null)
        {
            var context = await _storage.GetProcessExecutionContext(refId);
            var type = _parser.GetTypeByName(context.FlowName);
            context.ExecutionResult.FormLastAction = operationName;
            var runParameters = new FlowRunParameters { RefId = context.RefId, Model = model, FlowType = type, Context = context, NoStorageMode = false };

            if (flowParams != null)
            {
                runParameters.FlowParameters = flowParams;
            }

            return await ExecuteFlow(runParameters);
        }


        public async Task<IFlowContext> ContinueFlowNoStorage(IFlowContext context, string operationName = null, FlowParamsGeneric flowParams = null)
        {
            var type = _parser.GetTypeByName(context.FlowName);
            context.ExecutionResult.FormLastAction = operationName;
            var runParameters = new FlowRunParameters { RefId = context.RefId, FlowType = type, Context = context, NoStorageMode = true };

            if (flowParams != null)
            {
                runParameters.FlowParameters = flowParams;
            }

            return await ExecuteFlow(runParameters);
        }

        public async Task<IFlowContext> ContinueFlowNoStorage(ClientKeptContext context, IFlowModel model, FlowParamsGeneric flowParams)
        {
            throw new NotImplementedException();
        }

        public async Task<IFlowContext> CreateFlowContext(Type flowType, IFlowModel model = null, string currentTask = null, 
            FlowParamsGeneric flowParams = null)
        {
			var flowParameters = TypeHelper.GetConstructorParameters(_serviceProvider, flowType);
			var flow = Activator.CreateInstance(flowType, flowParameters) as IStateFlow;

			if (flow == null)
			{
				throw new FlowCreateException($"Flow {flowType} does not support IStateFlow interface");
			}

            flow.Parse();
			var context = await _storage.CreateProcessExecutionContext(flow, flowParams, true);
			context.ExecutionResult = new TaskExecutionResult();
			context.Model= model;


			if (currentTask != null)
            {
                context.CurrentTask = currentTask;
                context.CurrentTaskLine = flow.States.FindIndex(x => x.State == currentTask);
			}

			return context;
		}


		public async Task<IFlowContext> ExecuteFlow(FlowRunParameters runParameters)
        {
            var flowType = runParameters.FlowType;
            var refId = runParameters.RefId;
            var parameters = runParameters.FlowParameters;
            var noStorage = runParameters.NoStorageMode;
            var context = runParameters.Context;

            var flowParameters = TypeHelper.GetConstructorParameters(_serviceProvider, flowType);
            var flow = Activator.CreateInstance(flowType, flowParameters) as IStateFlow;

            if (flow == null)
            {
                throw new FlowCreateException($"Flow {flowType} does not support IStateFlow interface");
            }

            flow.Parse();

            if (context == null)
            {
                if (string.IsNullOrEmpty(refId))
                {
                    context = await _storage.CreateProcessExecutionContext(flow, parameters, noStorage);
                    context.ExecutionResult = new TaskExecutionResult();
                }
                else
                {
                    context = await _storage.GetProcessExecutionContext(refId);
                    context.ExecutionResult.FlowState = TaskExecutionFlowStateEnum.Continue;
                    context.ExecutionResult.ResultState = TaskExecutionResultStateEnum.Success;
                }

                context.ExecutionResult.CreatedDate = DateTime.Now;
            }
            else if (context != null && parameters != null)
            {
                // merge parameters from runParameters
                var di = context.Params.ConcatDynamic(parameters);
                context.Params = parameters;
                context.Params.DynamicInput = di;
            }

            // override model if it comes from parameters
            if (runParameters.Model != null)
            {
                context.Model = runParameters.Model;
            }

            //flow.SetFlowRefId(context.RefId);
            flow.SetFlowContext(context);

            var index = context.CurrentTaskLine;
            context.ExecutionResult.FlowState = TaskExecutionFlowStateEnum.Continue;
            context.ExecutionResult.ResultState = TaskExecutionResultStateEnum.Success;
            flow.SetParams(context.Params);
            flow.SetModel(context.Model);

            await Work();

            return context;

            async Task Work()
            {
                int i = index;
                int _currentIteration = 0;
                bool proceed = true;

                try
                {
                    // Execute flow begin
                    if (context.CurrentTaskLine == 0)
                    {
                        if (flow.OnBeginAsync != null)
                        {
                            await flow.OnBeginAsync.Invoke();
                        }
                    }

					// Execute current state begin
					if (flow.States[i].OnBeginAsync != null)
					{
						await flow.States[i].OnBeginAsync.Invoke();
					}

					while (proceed)
                    {
                        proceed = false;
                        _currentIteration++;

                        if (_currentIteration > MAX_LOOP_COUNT)
                        {
                            throw new FlowInfiniteExecutionException();
                        }

                        var task = flow.States[i];
                        context.CurrentTask = task.State;
                        context.CurrentTaskLine = i;
                        var transitions = flow.Transitions.Where(t => t.FromState == task.State).ToList();

                        foreach (var transition in transitions)
                        {
                            var trigger = transition.Trigger ?? transition.TriggerFunction();

                            if (trigger.IsTriggerAsync())
                            {
                                await trigger.CheckTriggerAsync(context);
                            }
                            else
                            {
                                trigger.CheckTrigger(context);
                            }

                            if (trigger.Proceed)
                            {
                                transition.OnChanging?.Invoke();

                                if (transition.OnChangingAsync != null)
                                {
                                    await transition.OnChangingAsync.Invoke();
                                }

                                i = flow.States.IndexOf(flow.States.First(s => s.State == transition.ToState));
                                proceed = true;
                                context.ExecutionResult.ResultState = TaskExecutionResultStateEnum.Success;
                                context.ExecutionResult.FlowState = TaskExecutionFlowStateEnum.Continue;
                                context.ExecutionResult.ChangedDate = DateTime.Now;

                                // execude begin
                                if (flow.States[i].OnBeginAsync != null)
                                {
                                    await flow.States[i].OnBeginAsync.Invoke();
                                }

                                // clean last action to prevent unexpected following propagation
                                context.ExecutionResult.FormLastAction = null;
                                break;
                            }
                        }
                    }

                    context.ExecutionResult.FlowState = TaskExecutionFlowStateEnum.Stop;

                    if (flow.States[i].IsEnd)
                    {
                        context.ExecutionResult.FlowState = TaskExecutionFlowStateEnum.Finished;
                        context.ExecutionResult.FinishedDate = DateTime.Now;
                    }

                }
                catch (Exception exc)
                {
                    context.ExecutionResult.ResultState = TaskExecutionResultStateEnum.Fail;
                    context.ExecutionResult.ExceptionMessage = exc.Message;
                    context.ExecutionResult.ExceptionStackTrace = exc.StackTrace;
                    context.ExecutionResult.ExecutionException = exc;
                }

                // save context
                if (noStorage)
                {
                }
                //else if (flow.Settings.StoreModel == FlowExecutionStoreModel.NoStoreTillStop && context.ExecutionResult.FlowState == TaskExecutionFlowStateEnum.Stop)
                //{
                //    await _storage.SaveProcessExecutionContext(context, context.ExecutionResult, true);
                //}
                else if (!noStorage)
                {
                    await _storage.SaveProcessExecutionContext(context, context.ExecutionResult, true);
                }
            }
        }

        public async Task<IFlowContext> ExecuteFlow(Type flowType, string refId, FlowParamsGeneric parameters)
        {
            throw new NotImplementedException();
        }

        public async Task<IFlowContext> ExecuteFlow(string flowType, string refId, FlowParamsGeneric parameters)
        {
            throw new NotImplementedException();
        }

        public async Task<IFlowContext> ExecuteFlowNoStorage(string flowType, FlowParamsGeneric parameters)
        {
            throw new NotImplementedException();
        }

        public async Task<UserViewModelPageResult> ExecuteFlowTask(string flowType, IFlowContext context, string userViewCallbackTaskName, QueryOptions queryOptions, dynamic dynamicParams = null)
        {
            throw new NotImplementedException();
        }

        public void UpdateOperation(string operationName)
        {
            throw new NotImplementedException();
        }

        public async Task<FlowDefinitionDetails> GetFlowDefinitionDetails(FlowRunParameters runParameters)
        {
            var result = new FlowDefinitionDetails();
            var flowType = runParameters.FlowType;
            var refId = runParameters.RefId;
            var parameters = runParameters.FlowParameters;
            var noStorage = runParameters.NoStorageMode;
            var model = runParameters.Model;
            var context = runParameters.Context;

            if (context == null && noStorage == false)
            {
                context = await _storage.GetProcessExecutionContext(refId);
            }

            var flowParameters = TypeHelper.GetConstructorParameters(_serviceProvider, flowType);
            var flow = Activator.CreateInstance(flowType, flowParameters) as IStateFlow;

            if (flow == null)
            {
                throw new FlowCreateException($"Flow {flowType} does not support IStateFlow interface");
            }

            flow.Parse();
            result.States = flow.States;
            result.Transitions = flow.Transitions;
            result.Forms = flow.Forms;
            result.CurrentState = context?.GetState();
            result.CurrentStateTransitions = flow.Transitions.Where(t => t.FromState == result.CurrentState).ToList();

            foreach (var transition in result.CurrentStateTransitions)
            {
                var trigger = transition.Trigger ?? transition.TriggerFunction();
                var selector = trigger.GetSelector();
                result.CurrentStateSelectors[transition] = selector;
            }

            return result;
        }

        public async Task UpdateFlowContextModel(string refId, IFlowModel model)
        {
            var context = await _storage.GetProcessExecutionContext(refId);
            context.Model = model;
            await _storage.SaveProcessExecutionContext(context, context.ExecutionResult);
        }

        public async Task UpdateFlowContext(IFlowContext context)
        {
            await _storage.SaveProcessExecutionContext(context, context.ExecutionResult);
        }

        public List<Type> GetAllFlowTypes()
        {
            var result = _parser.GetTypesInheritedFrom(typeof(IStateFlow)).Where(t => !t.IsAbstract && !t.ContainsGenericParameters).ToList();
            return result;
        }
    }
}
