using Microsoft.Extensions.Logging;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorForms.Flows.Definitions.Abstractions;
using Microsoft.Extensions.DependencyInjection;

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

                    if (context == null)
                    {
                        context = await _storage.CreateProcessExecutionContext(flow, parameters, true);
                        context.RefId = refId;
                        await _storage.SaveProcessExecutionContext(context, context.ExecutionResult, true);
                    }

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

            await RunFlowTasks(index, flow, context, noStorage);
            return context;
        }
        
        private static async Task ExecuteTask(TaskDef task, IFlow flow, IFlowContext context)
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

                context.Model = flow.GetModel();
                context.ExecutionResult.ResultState = TaskExecutionResultStateEnum.Success;
                context.ExecutionResult.FlowState = TaskExecutionFlowStateEnum.Continue;
            }
            catch (Exception exc)
            {
                context.ExecutionResult.ResultState = TaskExecutionResultStateEnum.Fail;
                context.ExecutionResult.ExceptionMessage = exc.Message;
                context.ExecutionResult.ExceptionStackTrace = exc.StackTrace;
                context.ExecutionResult.ExceptionType = exc.GetType().FullName;
                context.ExecutionResult.ExecutionException = exc;
            }
        }

        private async Task RunFlowTasks(int index, IFluentFlow flow, IFlowContext context, bool noStorage)
        {
            var i = index;
            var currentIteration = 0;

            while (i < flow.Tasks.Count)
            {
                if (context.ExecutionResult.ResultState == TaskExecutionResultStateEnum.Fail ||
                    context.ExecutionResult.FlowState == TaskExecutionFlowStateEnum.Stop ||
                    context.ExecutionResult.FlowState == TaskExecutionFlowStateEnum.Finished)
                {
                    // save context and stop if flow settings NoStoreTillStop
                    if (flow.Settings.StoreModel == FlowExecutionStoreModel.NoStoreTillStop &&
                        context.ExecutionResult.FlowState == TaskExecutionFlowStateEnum.Stop)
                    {
                        await _storage.SaveProcessExecutionContext(context, context.ExecutionResult, true);
                    }
                    else if (!noStorage)
                    {
                        await _storage.SaveProcessExecutionContext(context, context.ExecutionResult);
                    }

                    return;
                }

                currentIteration++;

                if (currentIteration > MAX_LOOP_COUNT)
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
                        await ExecuteTask(task, flow, context);
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
                        if (task.FormType != null)
                        {
                            // get an instance of the FormRulesCollection generic for this model, it must be registered in DI
                            var formRulesCollectionServices = _serviceProvider.GetServices(task.FormType);
                            var formRulesCollectionType = typeof(IFormRulesCollection<>).MakeGenericType(context.Model.GetType());
                            var formRulesCollection = formRulesCollectionServices
                                .FirstOrDefault(s => s?.GetType().GetInterfaces().Any(type => type == formRulesCollectionType) ?? false);
                            
                            var propertyInfo = formRulesCollectionType.GetProperty("Rules");

                            // Extract the rules from the property
                            var rulesObj = propertyInfo?.GetValue(formRulesCollection);
                            if (rulesObj is IEnumerable<Func<IFlowModel, Task<bool>>> rules)
                            {
                                var rulesArray = rules as Func<IFlowModel, Task<bool>>[] ?? rules.ToArray();
                                // Run the rules
                                for (var maxLoops = MAX_LOOP_COUNT; maxLoops > 0; maxLoops--)
                                {
                                    if (await RunFormRules(context, rulesArray))
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        
                        if (task.Action is not null)
                        {
                            await ExecuteTask(task, flow, context);
                        }

                        if (context.ExecutionResult.FormState != FormTaskStateEnum.Submitted &&
                            context.ExecutionResult.FormState != FormTaskStateEnum.Rejected)
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
                        await ExecuteTask(task, flow, context);
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

        private static async Task<bool> RunFormRules(IFlowContext context, IEnumerable<Func<IFlowModel, Task<bool>>> rules)
        {
            foreach (var ruleFunc in rules)
            {
                if (ruleFunc == null)
                {
                    continue;
                }

                try
                {
                    if (await ruleFunc(context.Model))
                    {
                        // return false to indicate that another iteration should be performed
                        return false;
                    }
                }
                catch (Exception exc)
                {
                    context.ExecutionResult.ResultState = TaskExecutionResultStateEnum.Fail;
                    context.ExecutionResult.ExceptionMessage = $"Rule {ruleFunc.Method.Name} execution failed: {exc.Message}";
                    context.ExecutionResult.ExceptionStackTrace = exc.StackTrace;
                    context.ExecutionResult.ExceptionType = exc.GetType().FullName;
                    context.ExecutionResult.ExecutionException = exc;
                    break;
                }
            }

            // return true to indicate that no more iterations are needed
            return true;
        }

        public async Task<UserViewModelPageResult> ExecuteFlowTask(string flowType, IFlowContext context, string userViewCallbackTaskName, QueryOptions queryOptions, dynamic dynamicParams = null)
        {
            return await FlowRunHelper.ExecuteFlowTask(_serviceProvider, _parser, flowType, context, userViewCallbackTaskName, queryOptions);
        }

        public List<Type> GetAllFlowTypes()
        {
            var result = _parser.GetTypesInheritedFrom(typeof(IFluentFlow)).Where(t => !t.IsAbstract && !t.ContainsGenericParameters).ToList();
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

        public async Task<FlowDefinitionDetails> GetFlowDefinitionDetails(FlowRunParameters runParameters)
        {
            var result = new FlowDefinitionDetails();
            var flowType = runParameters.FlowType;
            //var refId = runParameters.RefId;
            //var parameters = runParameters.FlowParameters;
            //var noStorage = runParameters.NoStorageMode;
            //var context = runParameters.Context;

            var flowParameters = TypeHelper.GetConstructorParameters(_serviceProvider, flowType);
            var flow = Activator.CreateInstance(flowType, flowParameters) as IFluentFlow;

            if (flow == null)
            {
                throw new FlowCreateException($"Flow {flowType} does not support IFluentFlow interface");
            }

            flow.Parse();
            var stateNames = new HashSet<string>();
            var index = -1;
            TaskDef task = null;
            //TaskDef prevTask = null;
            StateDef currentDef = null;
            StateDef prevDef = null;
            NextTask();

            while (index < flow.Tasks.Count)
            {
                ReadIfBlock();
                ReadTask();

                if (index == flow.Tasks.Count-1)
                {
                    break;
                }

                NextTask();
            }

            return result;

            void ReadTask()
            {
    //            if (task.Type == TaskDefTypes.Goto)
    //            {
				//	var newDef = GetTaskDef(task);
				//	result.States.Add(newDef);
				//	currentDef = newDef;

				//	if (prevDef != null)
				//	{
				//		var transition = GetTransitionDef(prevDef, currentDef);
				//		result.Transitions.Add(transition);
				//	}

    //                //var targetTask = flow.Tasks[task.GotoIndex];
    //                var labelDef = FindLabel(result, flow.Tasks[task.GotoIndex]);
				//	var gotoTransition = GetTransitionDef(currentDef, labelDef);
				//	result.Transitions.Add(gotoTransition);
				//}
    //            else 
                if (task.Type != TaskDefTypes.If && task.Type != TaskDefTypes.EndIf)
                {
                    var newDef = GetTaskDef(task);
                    result.States.Add(newDef);
                    currentDef = newDef;

                    if (prevDef != null)
                    {
                        var transition = GetTransitionDef(prevDef, currentDef);
                        result.Transitions.Add(transition);
                    }
                }

				if (task.Type == TaskDefTypes.Goto)
                {
					var labelDef = FindLabel(result, flow.Tasks[task.GotoIndex]);
					var gotoTransition = GetTransitionDef(currentDef, labelDef);
					result.Transitions.Add(gotoTransition);
                    prevDef = null;
                    currentDef = null;
				}
                else if (task.Type == TaskDefTypes.GotoIf)
				{
					var labelDef = FindLabel(result, flow.Tasks[task.GotoIndex]);
					var gotoTransition = GetTransitionDef(currentDef, labelDef);
					result.Transitions.Add(gotoTransition);
					//prevDef = null;
					//currentDef = null;
				}
			}

            void NextTask()
            {
                index++;
                //prevTask = task;
                prevDef = currentDef;
                task = flow.Tasks[index];
            }

            void ReadIfBlock() 
            { 
                if (task.Type != TaskDefTypes.If)
                {
                    return;
                }

                var startIfDef = GetTaskDef(task);
                result.States.Add(startIfDef);
                currentDef = startIfDef;

                if (prevDef != null)
                {
                    var transition = GetTransitionDef(prevDef, currentDef);
                    result.Transitions.Add(transition);
                }

                StateDef firstBranchEndDef = null;
                NextTask();

                while (task.Type != TaskDefTypes.EndIf)
                {
                    if (task.Type == TaskDefTypes.Else)
                    {
                        firstBranchEndDef = prevDef;
                        NextTask();
                        prevDef = startIfDef;
                    }
                    else
                    {
                        ReadIfBlock();
                        ReadTask();
                        NextTask();
                    }
                }

                //var startDef = GetTaskDef(startIf);
                var endDef = GetTaskDef(task);
                result.States.Add(endDef);
                currentDef = endDef;

                if (firstBranchEndDef != null)
                {
                    // add first branch to endIf
                    result.Transitions.Add(GetTransitionDef(firstBranchEndDef, endDef));
                }
                else
                {
					// add direct branch from if to endif
					result.Transitions.Add(GetTransitionDef(startIfDef, endDef));
				}

                // add second branch to endIf
                if (prevDef != null)
                {
                    result.Transitions.Add(GetTransitionDef(prevDef, endDef));
                }

                //NextTask();
            }

			StateDef FindLabel(FlowDefinitionDetails details, TaskDef task)
			{
                var result = details.States.FirstOrDefault(x => x.State == task.Name);

                if (result == null)
                {
                    result = new StateDef
                    {
                        State = $"{task.Index} : {task.Name}",
                        Type = task.Type.ToString(),
                    };
                }

				return result;
			}

			StateDef GetTaskDef(TaskDef task)
            {
                StateDef result;

				if (task.Type == TaskDefTypes.Goto || task.Type == TaskDefTypes.GotoIf)
                {
					result = new StateDef
					{
						State = GetAvailableName($"{task.Type.ToString()} {task.Name}"),
						//Caption = $"{task.Type.ToString()} {task.Name}",
						Type = task.Type.ToString(),
					};
				}
				else if (task.Type == TaskDefTypes.If || task.Type == TaskDefTypes.EndIf)
				{
					result = new StateDef
					{
						State = GetAvailableName($"{task.Index} : {task.Name}"),
						Type = task.Type.ToString(),
					};
				}
				else
                {
                    result = new StateDef
                    {
                        State = GetAvailableName($"{task.Index} : {task.Name}"),
                        //Caption = task.Name,
                        Type = task.Type.ToString(),
                    };
                }

                return result;
            }

            string GetAvailableName(string name)
            {
                if (stateNames.Contains(name))
                {
                    int i = 2;

                    while (stateNames.Contains($"{name}{i}"))
                    {
                        i++;
                    }

                    string newName = $"{name}{i}";
                    stateNames.Add(newName);
                    return newName;
                }

                stateNames.Add(name);
                return name;
            }

            TransitionDef GetTransitionDef(StateDef from, StateDef to)
            {
                var result = new TransitionDef
                {
                    FromState = from.State,
                    ToState = to.State,
                    Trigger = new TransitionTrigger { Text = "" },
                };

                return result;
            }
        }
    }
}
