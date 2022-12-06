using Microsoft.Extensions.Logging;
using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine;
using BlazorForms.Shared;
using BlazorForms.Shared.Exceptions;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows
{
    public class FluentFlowRunEngine: IFluentFlowRunEngine
    {
        public const int MAX_LOOP_COUNT = 1000;

        public event FlowEvent OnLoad;
        public event FlowEvent OnSave;

        protected readonly IFlowRunStorage _storage;
        protected readonly ILogger<FluentFlowRunEngine> _logger;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IFlowParser _parser;

        public FluentFlowRunEngine(ILogger<FluentFlowRunEngine> logger, IServiceProvider serviceProvider, IFlowRunStorage storage, IFlowParser parser)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _storage = storage;
            _parser = parser;
        }

        public void UpdateOperation(string operationName)
        {
            //empty
        }

        public async Task<IFlowContext> ContinueFlow(string refId, string operationName = null, FlowParamsGeneric flowParams = null)
        {
            var context = await _storage.GetProcessExecutionContext(refId);
            var type = _parser.GetTypeByName(context.FlowName);
            var runParameters = new FlowRunParameters { RefId = refId, FlowType = type, Context = context };

            if (flowParams != null)
            {
                runParameters.FlowParameters = flowParams;
            }

            return await ExecuteFlow(runParameters);
        }

        public async Task<IFlowContext> ContinueFlowNoStorage(IFlowContext context, string operationName = null, FlowParamsGeneric flowParams = null)
        {
            var type = _parser.GetTypeByName(context.FlowName);
            var runParameters = new FlowRunParameters { RefId = context.RefId, FlowType = type, Context = context, NoStorageMode = true };

            if (flowParams != null)
            {
                runParameters.FlowParameters = flowParams;
            }

            return await ExecuteFlow(runParameters);
        }
        public async Task<IFlowContext> ContinueFlowNoStorage(ClientKeptContext ctx, IFlowModel model, FlowParamsGeneric flowParams)
        {
            var context = new FlowContext(ctx, model);
            var type = _parser.GetTypeByName(context.FlowName);
            var runParameters = new FlowRunParameters { RefId = context.RefId, FlowType = type, Context = context, NoStorageMode = true };

            if (flowParams != null)
            {
                runParameters.FlowParameters = flowParams;
            }

            return await ExecuteFlow(runParameters);
        }

        #region ExecuteFlow overloads
        public async Task<IFlowContext> ExecuteFlow(Type flowType, string refId, FlowParamsGeneric parameters)
        {
            var runParameters = new FlowRunParameters
            {
                FlowType = flowType,
                RefId = refId,
                FlowParameters = parameters
            };

            return await ExecuteFlow(runParameters);
        }

        public async Task<IFlowContext> ExecuteFlow(string flowType, string refId, FlowParamsGeneric parameters)
        {
            var type = _parser.GetTypeByName(flowType);

            if (type == null)
            {
                throw new InvalidOperationException($"Flow type {flowType} not found, are you missing an assembly reference?");
            }

            return await ExecuteFlow(type, refId, parameters);
        }

        public async Task<IFlowContext> ExecuteFlowNoStorage(string flowType, FlowParamsGeneric parameters)
        {
            var type = _parser.GetTypeByName(flowType);

            var runParameters = new FlowRunParameters
            {
                FlowType = type,
                FlowParameters = parameters,
                NoStorageMode = true,
                FirstPass = parameters?.DynamicInput.ContainsKey(FlowConstants.SecondPass) == false
            };

            return await ExecuteFlow(runParameters);
        }
        #endregion

        public virtual async Task<IFlowContext> ExecuteFlow(FlowRunParameters runParameters)
        {
            var flowType = runParameters.FlowType;
            var refId = runParameters.RefId;
            var parameters = runParameters.FlowParameters;
            var noStorage = runParameters.NoStorageMode;
            var context = runParameters.Context;

            var flowParameters = TypeHelper.GetConstructorParameters(_serviceProvider, flowType);
            var flow = Activator.CreateInstance(flowType, flowParameters) as IFluentFlow;

            if (flow == null)
            {
                throw new FlowCreateException($"Flow {flowType} does not support IFluentFlow interface");
            }

            flow.Parse();
            flow.SetFirstPass(runParameters.FirstPass);

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
            }
            else if (context != null && parameters != null)
            {
                // merge parameters from runParameters
                var di = context.Params.ConcatDynamic(parameters);
                context.Params = parameters;
                context.Params.DynamicInput = di;
            }

            flow.SetFlowRefId(context.RefId);
            flow.SetFlowContext(context);

            var index = context.CurrentTaskLine;
            context.ExecutionResult.FlowState = TaskExecutionFlowStateEnum.Continue;
            context.ExecutionResult.ResultState = TaskExecutionResultStateEnum.Success;
            flow.SetParams(context.Params);
            flow.SetModel(context.Model);

            await Work();
            return context;

            async Task ExecuteTask(TaskDef task)
            {
                try
                {
                    if (task.Action != null)
                    {
                        await task.Action();
                    }
                    else if (task.NonAsyncAction != null)
                    {
                        task.NonAsyncAction();
                    }

                    context.ExecutionResult.ResultState = TaskExecutionResultStateEnum.Success;
                    context.ExecutionResult.FlowState = TaskExecutionFlowStateEnum.Continue;
                }
                catch (Exception exc)
                {
                    context.ExecutionResult.ResultState = TaskExecutionResultStateEnum.Fail;
                    context.ExecutionResult.ExceptionMessage = exc.Message;
                    context.ExecutionResult.ExceptionStackTrace = exc.StackTrace;
                    context.ExecutionResult.ExecutionException = exc;
                }
            }

            async Task Work()
            {
                int i = index;
                int _currentIteration = 0;

                while (i < flow.Tasks.Count)
                {
                    if(context.ExecutionResult.ResultState == TaskExecutionResultStateEnum.Fail ||
                       context.ExecutionResult.FlowState == TaskExecutionFlowStateEnum.Stop ||
                       context.ExecutionResult.FlowState == TaskExecutionFlowStateEnum.Finished )
                    {
                        // save context and stop if flow settings NoStoreTillStop
                        if (flow.Settings.StoreModel == FlowExecutionStoreModel.NoStoreTillStop && context.ExecutionResult.FlowState == TaskExecutionFlowStateEnum.Stop)
                        {
                            await _storage.SaveProcessExecutionContext(context, context.ExecutionResult, true);
                        }
                        else if (!noStorage)
                        {
                            await _storage.SaveProcessExecutionContext(context, context.ExecutionResult);
                        }

                        return;
                    }

                    _currentIteration++;

                    if(_currentIteration > MAX_LOOP_COUNT)
                    {
                        throw new FlowInfiniteExecutionException();
                    }

                    var task = flow.Tasks[i];
                    context.CurrentTask = task.Name;
                    context.CurrentTaskLine = i;

                    switch (task.Type)
                    {
                        case TaskDefTypes.Goto:
                            i = task.GotoIndex;
                            continue;

                        case TaskDefTypes.GotoIf:
                            if (task.Condition())
                            {
                                i = task.GotoIndex;
                            }

                            i++;
                            continue;

                        case TaskDefTypes.Label:
                            i++;
                            continue;

                        case TaskDefTypes.Begin:
                        case TaskDefTypes.Task:
                            await ExecuteTask(task);
                            i++;
                            continue;

                        case TaskDefTypes.Wait:

                            if (task.Condition())
                            {
                                // stop execution and wait until the condition is met
                                context.ExecutionResult.ResultState = TaskExecutionResultStateEnum.Success;
                                context.ExecutionResult.FlowState = TaskExecutionFlowStateEnum.Stop;
                                context.ExecutionResult.IsWaitTask = true;
                            }
                            else
                            {
                                context.ExecutionResult.IsWaitTask = false;
                                i++;
                            }

                            continue;

                        case TaskDefTypes.Form:
                            if (context.ExecutionResult.FormState != FormTaskStateEnum.Submitted && context.ExecutionResult.FormState != FormTaskStateEnum.Rejected)
                            {
                                // stop execution and wait for Form submit
                                context.ExecutionResult.ResultState = TaskExecutionResultStateEnum.Success;
                                context.ExecutionResult.FlowState = TaskExecutionFlowStateEnum.Stop;
                                context.ExecutionResult.FormId = task.FormType?.FullName ?? task.FormTypeName;
                                context.ExecutionResult.IsFormTask = true;
                                context.ExecutionResult.CallbackTaskId = task.CallbackTask;
                                context.ExecutionResult.PreloadTableData = task.PreloadTableData;
                            }
                            else
                            {
                                // form submitted - goto next task
                                context.ExecutionResult.ResultState = TaskExecutionResultStateEnum.Success;
                                context.ExecutionResult.FlowState = TaskExecutionFlowStateEnum.Continue;
                                context.ExecutionResult.IsFormTask = false;
                                context.ExecutionResult.FormState = FormTaskStateEnum.Initialized;
                                i++;
                            }

                            continue;

                        case TaskDefTypes.End:
                            await ExecuteTask(task);
                            context.ExecutionResult.FlowState = TaskExecutionFlowStateEnum.Finished;
                            continue;

                        case TaskDefTypes.If:
                            if (task.Condition())
                            {
                                i++;
                            }
                            else
                            {
                                // goto Else/EndIf
                                i = task.GotoIndex;
                            }

                            continue;

                        case TaskDefTypes.Else:
                            // meet this operator only after passing all tasks inside if { ... } body, so use goto to EndIf
                            i = task.GotoIndex;
                            continue;

                        case TaskDefTypes.EndIf:
                            i++;
                            continue;
                    }
                }
            }
        }

        public async Task<UserViewModelPageResult> ExecuteFlowTask(string flowType, IFlowContext context, string userViewCallbackTaskName, QueryOptions queryOptions, dynamic dynamicParams = null)
        {
            return await FlowRunHelper.ExecuteFlowTask(_serviceProvider, _parser, flowType, context, userViewCallbackTaskName, queryOptions);
        }

        public List<Type> GetAllFlowTypes()
        {
            var result = _parser.GetTypesInheritedFrom(typeof(IFluentFlow)).Where(t => !t.IsAbstract).ToList();
            return result;
        }

        public async Task UpdateFlowContextModel(string refId, IFlowModel model)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateFlowContext(IFlowContext context)
        {
            throw new NotImplementedException();
        }
    }
}
