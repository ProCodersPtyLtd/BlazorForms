using BlazorForms.FlowRules;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BlazorForms.Proxyma;
using BlazorForms.Platform.Shared.Attributes;
using Microsoft.Extensions.Logging;
using BlazorForms.Flows;
using BlazorForms.Shared.Extensions;
using BlazorForms.Platform.Definitions.Shared;
using BlazorForms.Rendering;
using BlazorForms.Shared.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorForms.Platform.ProcessFlow
{
    // ToDo: should be covered by unit test
    public class FlowRunProvider : IFlowRunProvider
    {
        private readonly ILogger _logger;
        private readonly IFlowRunStorage _storage;
        private readonly IFormDefinitionParser _formDefinitionParser;
        private readonly IRuleDefinitionParser _ruleDefinitionParser;
        // ToDo: decide how to supply assembly list, that are considered in type resolution
        private readonly IFlowParser _flowParser;
        // private readonly IFlowRunEngine _processingEngine;
        private readonly IAssemblyRegistrator _assemblyRegistrator;
        private readonly IProxymaProvider _modelProxyProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserViewDataResolver _userViewDataResolver;
        private readonly IJsonPathNavigator _jsonPathNavigator;
        private readonly ICustomConfigProvider _customConfigProvider;
        private readonly IAuthState _authState;
        private readonly IFluentFlowRunEngine _fluentFlowRunEngine;
        private readonly ILogStreamer _logStreamer;
        private readonly IClientBrowserService _clientBrowserService;
        private readonly IRuleExecutionEngine _ruleEngine;
        
        // ToDo: StackOverflow thrown if we create ruleEngine through DI
        //private readonly IRuleExecutionEngine _ruleExecutionEngine;

        public FlowRunProvider(
            ILogger<FlowRunProvider> logger,
            IFlowRunStorage storage,
            IFormDefinitionParser formDefinitionParser,
            IRuleDefinitionParser ruleDefinitionParser,
            IFlowParser flowParser,
            //IFlowRunEngine processingEngine,
            IAssemblyRegistrator assemblyRegistrator,
            IProxymaProvider modelProxyProvider,
            IServiceProvider serviceProvider,
            IUserViewDataResolver userViewDataResolver,
            ICustomConfigProvider customConfigProvider,
            IAuthState authState,
            IFluentFlowRunEngine fluentFlowRunEngine,
            ILogStreamer logStreamer,
            IClientBrowserService clientBrowserService
            )
        {
            _logger = logger;
            _storage = storage;
            _formDefinitionParser = formDefinitionParser;
            _flowParser = flowParser;
            //_processingEngine = processingEngine;
            _ruleDefinitionParser = ruleDefinitionParser;
            _assemblyRegistrator = assemblyRegistrator;
            _modelProxyProvider = modelProxyProvider;
            _serviceProvider = serviceProvider;
            _userViewDataResolver = userViewDataResolver;
            _jsonPathNavigator = _serviceProvider.GetService(typeof(IJsonPathNavigator)) as IJsonPathNavigator;
            _customConfigProvider = customConfigProvider;
            _authState = authState;
            _fluentFlowRunEngine = fluentFlowRunEngine;
            _ruleEngine = _serviceProvider.GetService<IRuleExecutionEngine>();
            // ToDo: StackOverflow thrown if we create ruleEngine through _serviceProvider
            //_ruleExecutionEngine = _serviceProvider.GetService(typeof(IRuleExecutionEngine)) as IRuleExecutionEngine;

            //_processingEngine.OnLoad += ProcessingEngine_OnLoad;
            //_processingEngine.OnSave += ProcessingEngine_OnSave;

            _fluentFlowRunEngine.OnLoad += ProcessingEngine_OnLoad;
            _fluentFlowRunEngine.OnSave += ProcessingEngine_OnSave;
            _logStreamer = logStreamer;
            _clientBrowserService = clientBrowserService;
        }

        private void ProcessingEngine_OnSave(object sender, FlowEventArgs e)
        {
            _customConfigProvider.SaveCustomData(e);
        }

        private void ProcessingEngine_OnLoad(object sender, FlowEventArgs e)
        {
            _customConfigProvider.LoadCustomData(e);
        }

        private async Task<IFlowRunEngine> GetFlowRunEngine(string flowType)
        {
            return _fluentFlowRunEngine;
        }

        #region Submit
        public async Task SaveForm(string refId, IFlowModel model, string actionBinding = null, string operationName = null)
        {
            var context = await GetFlowRunExecutionContext(refId);

            var engine = await GetFlowRunEngine(context.FlowName);
            engine.UpdateOperation(operationName);

            context.CallStack.Add(context.CurrentTask);
            context.Model = model;
            var result = context.ExecutionResult;
            //result.FormState = FormTaskStateEnum.Submitted;
            result.FormLastAction = actionBinding;
            await _storage.SaveProcessExecutionContext(context, result);
        }

        public async Task FinishFlow(string refId, IFlowModel model, string actionBinding = null, string operationName = null)
        {
            var context = await GetFlowRunExecutionContext(refId);

            var engine = await GetFlowRunEngine(context.FlowName);
            engine.UpdateOperation(operationName);

            context.CallStack.Add(context.CurrentTask);
            context.Model = model;
            var result = context.ExecutionResult;
            result.FlowState = TaskExecutionFlowStateEnum.Finished;
            result.FormLastAction = actionBinding;
            await _storage.SaveProcessExecutionContext(context, result);
        }

        public async Task<IFlowContext> SubmitForm(string refId, IFlowModel model, string actionBinding = null, string operationName = null, FlowParamsGeneric flowParams = null)
        {
            var context = await GetFlowRunExecutionContext(refId);
            context.CallStack.Add(context.CurrentTask);
            context.Model = model;
            var result = context.ExecutionResult;
            result.FormState = FormTaskStateEnum.Submitted;
            result.FormLastAction = actionBinding;
            await _storage.SaveProcessExecutionContext(context, result);

            var resultContext = await _fluentFlowRunEngine.ContinueFlow(refId, operationName, flowParams);
            return resultContext;
        }

        #endregion

        #region Reject
        public async Task<IFlowContext> RejectForm(string refId, IFlowModel model, string actionBinding = null, string operationName = null)
        {
            var context = await GetFlowRunExecutionContext(refId);
            context.CallStack.Add(context.CurrentTask);
            context.Model = model;
            var result = context.ExecutionResult;
            result.FormState = FormTaskStateEnum.Rejected;
            result.FormLastAction = actionBinding;
            await _storage.SaveProcessExecutionContext(context, result);
            var engine = await GetFlowRunEngine(context.FlowName);
            var resultContext = await engine.ContinueFlow(refId, operationName);
            return resultContext;
        }

        #endregion

        #region Get forms

        public async Task<IFlowContext> SubmitListItemForm(string refId, IFlowModel model, string operationName = null)
        {
            var resultContext = await SubmitForm(refId, model, null, operationName);

            if(resultContext.ExecutionResult.IsFormTask && resultContext.ExecutionResult.ResultState == TaskExecutionResultStateEnum.Success)
            {
                // ignore flow that stopped on another form
                return resultContext;
            }

            if(resultContext.ExecutionResult.FlowState != TaskExecutionFlowStateEnum.Finished || resultContext.ExecutionResult.ResultState != TaskExecutionResultStateEnum.Success)
            {
                _logStreamer.TrackException(new Exception(resultContext.ExecutionResult.ExceptionMessage));
                throw new Exception(resultContext.ExecutionResult.ExceptionMessage);
            }

            return resultContext;
        }

        public async Task<IUserViewModel> GetFlowDefaultReadonlyView(string refId)
        {
            var context = await GetFlowRunExecutionContext(refId);
            var formType = _flowParser.GetDefaultReadonlyForm(context.FlowName);

            var result = TypeHelper.GetGenericInstance<IUserViewModel>(typeof(UserViewModel<>), context.Model);
            result.SetModel(context.Model, context.Params);
            var formDetails = _formDefinitionParser.Parse(formType);
            result.UserViewDetails = formDetails;

            var ruleDetails = _ruleDefinitionParser.Parse(formType);

            foreach (var ctrl in formDetails.Fields)
            {
                // for fluent form resolve Rule Code
                if (ctrl.FlowRules.Any())
                {
                    ResolveRuleCodes(ctrl);
                }
                else
                {
                    // for legacy forms read from attributes
                    var fildWithRules = ruleDetails.Fields.FirstOrDefault(f => f.Binding.TemplateKey == ctrl.Binding.TemplateKey);

                    if (fildWithRules == null)
                    {
                        ctrl.FlowRules = new Collection<FormFlowRuleDetails>();
                    }
                    else
                    {
                        ctrl.FlowRules = new Collection<FormFlowRuleDetails>(fildWithRules.Rules.Select(
                            r => new FormFlowRuleDetails
                            {
                                FormRuleCode = r.RuleCode,
                                FormRuleType = r.RuleType.GetDescription(),
                                FormRuleTriggerType = r.RuleTriggerType.GetDescription(),
                            }).ToList());
                    }
                }
            }

            _customConfigProvider.DecorateForm(formDetails);
            return result;
        }

        private void ResolveRuleCodes(FieldControlDetails ctrl)
        {
            if (ctrl.FlowRules.Any())
            {
                foreach (var r in ctrl.FlowRules)
                {
                    if (r.FormRuleCode == null)
                    {
                        var ruleType = _flowParser.GetTypeByName(r.FormRuleType);
                        var rule = _ruleDefinitionParser.CreateRuleInstance(ruleType);
                        r.FormRuleCode = rule.RuleCode;
                    }
                }
            }
        }

        public async Task<IUserViewModel> GetListItemFlowUserView(string flowType, FlowParamsGeneric parameters)
        {
            var context = await _fluentFlowRunEngine.ExecuteFlow(flowType, null, await UpdateParameters(parameters));

            if (context.ExecutionResult.IsFormTask)
            {
                var result = TypeHelper.GetGenericInstance<IUserViewModel>(typeof(UserViewModel<>), context.Model);
                result.SetModel(context.Model, context.Params);
                result.RefId = context.RefId;
                var formType = _flowParser.GetTypeByName(context.ExecutionResult.FormId);
                var formDetails = _formDefinitionParser.Parse(formType);
                result.UserViewDetails = formDetails;

                var ruleDetails = _ruleDefinitionParser.Parse(formType);

                foreach (var ctrl in formDetails.Fields)
                {
                    // for fluent form resolve Rule Code
                    if (ctrl.FlowRules.Any())
                    {
                        ResolveRuleCodes(ctrl);
                    }
                    else
                    {
                        // for legacy forms read from attributes
                        var fildWithRules = ruleDetails.Fields.FirstOrDefault(f => f.Binding.TemplateKey == ctrl.Binding.TemplateKey);

                        if (fildWithRules == null)
                        {
                            ctrl.FlowRules = new Collection<FormFlowRuleDetails>();
                        }
                        else
                        {
                            ctrl.FlowRules = new Collection<FormFlowRuleDetails>(fildWithRules.Rules.Select(
                                r => new FormFlowRuleDetails
                                {
                                    FormRuleCode = r.RuleCode,
                                    FormRuleType = r.RuleType.GetDescription(),
                                    FormRuleTriggerType = r.RuleTriggerType.GetDescription(),
                                }).ToList());
                        }
                    }
                }

                _customConfigProvider.DecorateForm(formDetails);
                return result;
            }

            _logStreamer.TrackException(new InvalidStateException($"Flow with name={flowType} is not in FormTask state"));
            throw new InvalidStateException($"Flow with name={flowType} is not in FormTask state");
        }

        public async Task<IUserViewModel> GetListFlowUserView(string flowType, FlowParamsGeneric parameters, QueryOptions queryOptions)
        {
            var context = await _fluentFlowRunEngine.ExecuteFlowNoStorage(flowType, await UpdateParameters(parameters));

            if (context.ExecutionResult.IsFormTask)
            {
                var formTaskDetails = new FlowRunUserViewDetails
                {
                    UserViewName = context.ExecutionResult.FormId,
                    UserViewCallbackTaskName = context.ExecutionResult.CallbackTaskId
                };

                // retrieve data
                var _queryOptions = queryOptions ?? GetDefaultQueryOptions();
                UserViewModelPageResult executionResult;

                using (var wt = new WatchTracer(_logger, "ExecuteFlowTask - read data", _logStreamer))
                {
                    executionResult = await _fluentFlowRunEngine.ExecuteFlowTask(flowType, context, formTaskDetails.UserViewCallbackTaskName,
                        _queryOptions, null);
                }

                var result = TypeHelper.GetGenericInstance<IUserViewModel>(typeof(UserViewModel<>), executionResult.Model);
                result.RefId = context.RefId;
                result.AccessInfo = new UserViewAccessInformation { AdminUser = context.AdminUser, AssignedUser = context.AssignedUser, AssignedTeam = context.AssignedTeam };
                result.QueryOptions = queryOptions;

                using (var wt = new WatchTracer(_logger, "Parse Form and ResolveData", _logStreamer))
                {
                    result.SetModel(executionResult.Model, context.Params);
                    var formType = _flowParser.GetTypeByName(context.ExecutionResult.FormId);
                    result.UserViewDetails = _formDefinitionParser.Parse(formType);

                    if (executionResult.MethodTaskAttributes.Any(a => a.GetType() == typeof(ResolveTableDataAttribute)) ||
                        context.ExecutionResult.PreloadTableData)
                    {
                        var rawData = _userViewDataResolver.ResolveData(result.UserViewDetails, executionResult.Model, _logStreamer);
                        ParseRawData(result, rawData);
                    }

                    foreach (var ctrl in result.UserViewDetails.Fields)
                    {
                        // for fluent form resolve Rule Code
                        if (ctrl.FlowRules.Any())
                        {
                            ResolveRuleCodes(ctrl);
                        }
                    }
                }

                _customConfigProvider.DecorateForm(result.UserViewDetails);
                return result;
            }

            _logStreamer.TrackException(new InvalidStateException($"Flow with name={flowType} is not in FormTask state"));
            throw new InvalidStateException($"Flow with name={flowType} is not in FormTask state");
        }

        private void ParseRawData(IUserViewModel result, string[,] arr)
        {
            int y = arr.GetLength(0);
            int x = arr.GetLength(1);


            result.RawDataWidth = arr.GetLength(1);
            result.RawDataList = new List<string>();

            for (int j = 0; j < y; j++)
            {

                for (int i = 0; i < x; i++)
                {
                    result.RawDataList.Add(arr[j, i]);
                }
            }
        }

        private QueryOptions GetDefaultQueryOptions()
        {
            return new QueryOptions { PageIndex = 0, PageSize = 25 };
        }

        public async Task<FlowRunUserViewDetails> GetCurrentFlowRunUserView(string refId, IFlowContext flowContext = null)
        {
            var result = new FlowRunUserViewDetails();
            // ToDo: can we try to read from History?
            var context = flowContext ?? await _storage.GetProcessExecutionContext(refId);

            if(context.ExecutionResult.IsFormTask)
            {
                result.UserViewName = context.ExecutionResult.FormId;
                result.UserViewCallbackTaskName = context.ExecutionResult.CallbackTaskId;
                return result;
            }

            var exDetails = $"{context.ExecutionResult?.ExceptionMessage}\r\n{context.ExecutionResult?.ExceptionStackTrace}";
            _logStreamer.TrackException(new InvalidStateException($"Flow with ref id {refId} is not in FormTask state.\r\n{exDetails}"));
            throw new InvalidStateException($"Flow with ref id {refId} is not in FormTask state.\r\n{exDetails}");
        }

        // ToDo: should be covered by unit test
        public async Task<FormDetails> GetFormDetails(string formName)
        {
            var formType =_flowParser.GetTypeByName(formName);
            var formDetails = _formDefinitionParser.Parse(formType);
            var ruleDetails = _ruleDefinitionParser.Parse(formType);

            foreach(var ctrl in formDetails.Fields)
            {
                // for fluent form resolve Rule Code
                if (ctrl.FlowRules.Any())
                {
                    ResolveRuleCodes(ctrl);
                }
                else
                {
                    // for legacy forms read from attributes
                    var fildWithRules = ruleDetails.Fields.FirstOrDefault(f => f.Binding.TemplateKey == ctrl.Binding.TemplateKey);

                    if (fildWithRules == null)
                    {
                        ctrl.FlowRules = new Collection<FormFlowRuleDetails>();
                    }
                    else
                    {
                        ctrl.FlowRules = new Collection<FormFlowRuleDetails>(fildWithRules.Rules.Select(
                            r => new FormFlowRuleDetails
                            {
                                FormRuleCode = r.RuleCode,
                                FormRuleType = r.RuleType.GetDescription(),
                                FormRuleTriggerType = r.RuleTriggerType.GetDescription(),
                                IsOuterProperty = r.IsOuterProperty
                            }).ToList());
                    }
                }
            }

            _customConfigProvider.DecorateForm(formDetails);
            return formDetails;
        }
        #endregion

        #region model
        public async Task<IFlowModel> GetLastModel(string refId)
        {
            var model = await _storage.GetFlowModelByRef(refId);
            return model;
        }

        #endregion

        #region rules

        public async Task<RuleEngineExecutionResult> ExecuteFormLoadRules(RuleExecutionRequest ruleRequest, IFlowModel model)
        {
            try
            {
                // ToDo: why we create rule engine here? use DI
                var pa = _serviceProvider.GetService(typeof(IRuleDefinitionParser)) as IRuleDefinitionParser;
                
                //var engine = new InterceptorBasedRuleEngine(pa, _assemblyRegistrator, _modelProxyProvider, _jsonPathNavigator, _logStreamer,
                //    _serviceProvider.GetService<IKnownTypesBinder>());

                var engine = _ruleEngine;

                var dictionary = ruleRequest.DisplayProperties.Select(p => new DisplayDetails
                {
                    Caption = p.Caption,
                    Disabled = p.Disabled.Value,
                    Highlighted = p.Highlighted.Value,
                    Binding = p.Binding,
                    Name = p.Name,
                    Hint = p.Hint,
                    Required = p.Required.Value,
                    Visible = p.Visible.Value,
                }).ToDictionary(p => p.Binding.Key, p => p);

                var loadParameters = new RuleExecutionParameters
                {
                    Model = model,
                    TriggeredTriggerType = FormRuleTriggers.Loaded,
                    ProcessTaskTypeFullName = ruleRequest.ProcessTaskTypeFullName,
                    FlowParams = ruleRequest.FlowParams,
                    Fields = ruleRequest.Fields,
                    FieldsDisplayProperties = dictionary
                };

                var loadResult = await engine.Execute(loadParameters);
                return loadResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ExecuteFormLoadRules failed: {ex.Message}");
                throw;
            }

        }

        public async Task<bool> CheckFormAccess(FormAccessDetails access, IFlowContext context)
        {
            if (access == null || access.OnlyAssignee)
            {
                // TODO: Validate this!
                var userLogin = await _authState.UserLogin();
                return  string.IsNullOrEmpty(context.AssignedUser) ||
                        (!string.IsNullOrEmpty(userLogin) && userLogin == context.AssignedUser) ||
                        (!string.IsNullOrEmpty(context.AdminUser) && !string.IsNullOrEmpty(userLogin) && context.AdminUser == userLogin);
            }

            if (!string.IsNullOrWhiteSpace(access.Roles))
            {
                var roles = access.Roles.Split(new char[] { ',', ';' });
                throw new NotImplementedException();
            }

            if (access.CustomRule != null)
            {
                var accessModel = new AccessRuleModel { AssignedTeam = context.AssignedTeam, AssignedUser = context.AssignedUser };
                var parameters = new RuleExecutionParameters { Model = context.Model, FlowParams = context.Params, AccessModel = accessModel, TriggeredRuleCode = access.CustomRule.FormRuleCode };
                var pa = _serviceProvider.GetService(typeof(IRuleDefinitionParser)) as IRuleDefinitionParser;
                var engine = new AccessRuleEngine(pa, _assemblyRegistrator, _modelProxyProvider, _jsonPathNavigator);
                var execResult = await engine.Execute(parameters);
                return execResult.AccessModel.Allow;
            }

            return true;
        }

        public async Task<bool> CheckFormAccess(FormAccessDetails access, UserViewAccessInformation accessDetails, IFlowModel model, FlowParamsGeneric flowParams)
        {
            if (access == null || access.OnlyAssignee)
            {
                // TODO: Validate this!
                var userLogin = await _authState.UserLogin();
                return string.IsNullOrEmpty(accessDetails.AssignedUser) ||
                        (!string.IsNullOrEmpty(userLogin) && userLogin == accessDetails.AssignedUser) ||
                        (!string.IsNullOrEmpty(accessDetails.AdminUser) && !string.IsNullOrEmpty(userLogin) && accessDetails.AdminUser == userLogin);
            }

            if (!string.IsNullOrWhiteSpace(access.Roles))
            {
                var roles = access.Roles.Split(new char[] { ',', ';' });
                throw new NotImplementedException();
            }

            if (access.CustomRule != null)
            {
                var accessModel = new AccessRuleModel { AssignedTeam = accessDetails.AssignedTeam, AssignedUser = accessDetails.AssignedUser };
                var parameters = new RuleExecutionParameters { Model = model, FlowParams = flowParams, AccessModel = accessModel, TriggeredRuleCode = access.CustomRule.FormRuleCode };
                var pa = _serviceProvider.GetService(typeof(IRuleDefinitionParser)) as IRuleDefinitionParser;
                var engine = new AccessRuleEngine(pa, _assemblyRegistrator, _modelProxyProvider, _jsonPathNavigator);
                var execResult = await engine.Execute(parameters);
                return execResult.AccessModel.Allow;
            }

            return true;
        }

        public async Task<RuleEngineExecutionResult> TriggerRule(RuleExecutionRequest ruleRequest, IFlowModel model)
        {
            // ToDo: why we create rule engine here? use DI
            var pa = _serviceProvider.GetService(typeof(IRuleDefinitionParser)) as IRuleDefinitionParser;

            //var engine = new InterceptorBasedRuleEngine(pa, _assemblyRegistrator, _modelProxyProvider, _jsonPathNavigator, _logStreamer,
            //    _serviceProvider.GetService<IKnownTypesBinder>());

            var engine = _ruleEngine;

            var dictionary = ruleRequest.DisplayProperties.Select(p => new DisplayDetails
            {
                Caption = p.Caption,
                Disabled = p.Disabled.Value,
                Highlighted = p.Highlighted.Value,
                Binding = p.Binding,
                Name = p.Name,
                Hint = p.Hint,
                Required = p.Required.Value,
                Visible = p.Visible.Value,
            }).ToDictionary(p => p.Binding.Key, p => p);

            var parameters = new RuleExecutionParameters
            {
                Model = model,
                TriggeredTriggerType = ruleRequest.Trigger,
                TriggeredRuleCode = ruleRequest.RuleCode,
                TriggeredFieldBinding = ruleRequest.FieldBinding,
                ProcessTaskTypeFullName = ruleRequest.ProcessTaskTypeFullName,
                Fields = ruleRequest.Fields,
                FieldsDisplayProperties = dictionary,
                RowIndex = ruleRequest.RowIndex,
                FlowParams = ruleRequest.FlowParams
            };
            try
            {
                var execResult = await engine.Execute(parameters);
                return execResult;
            }
            catch (Exception ex)
            {
                // TODO: Add exceptions to RuleEngineExecutionResult
                //return new RuleEngineExecutionResult { Model = model };
                throw;
            }
        }


        public async Task<RuleEngineExecutionResult> TriggerRule(string processTaskTypeFullName, IFlowModel model, FlowParamsGeneric flowParams, 
            FieldDisplayDetails[] displayProperties, FormRuleTriggers? trigger,
            string ruleCode, FieldBinding fieldBinding, int rowIndex = 0)
        {
            // ToDo: why we create rule engine here? use DI
            var pa = _serviceProvider.GetService(typeof(IRuleDefinitionParser)) as IRuleDefinitionParser;

            //var engine = new InterceptorBasedRuleEngine(pa, _assemblyRegistrator, _modelProxyProvider, _jsonPathNavigator, _logStreamer,
            //    _serviceProvider.GetService<IKnownTypesBinder>());

            var engine = _ruleEngine;

            var dictionary = displayProperties.Select(p => new DisplayDetails
            {
                Caption = p.Caption,
                Disabled = p.Disabled.Value,
                Highlighted = p.Highlighted.Value,
                Binding = p.Binding,
                Name = p.Name,
                Hint = p.Hint,
                Required = p.Required.Value,
                Visible = p.Visible.Value,
            }).ToDictionary(p => p.Binding.Key, p => p);

            var parameters = new RuleExecutionParameters
            {
                Model = model,
                TriggeredTriggerType = trigger,
                TriggeredRuleCode = ruleCode,
                TriggeredFieldBinding = fieldBinding,
                ProcessTaskTypeFullName = processTaskTypeFullName,
                FieldsDisplayProperties = dictionary,
                RowIndex = rowIndex,
                FlowParams = flowParams
            };
            try
            {
                var execResult = await engine.Execute(parameters);
                return execResult;
            }
            catch (Exception ex)
            {
                // TODO: Add exceptions to RuleEngineExecutionResult
                return new RuleEngineExecutionResult { Model = model };
            }
        }
        #endregion

        public IEnumerable<string> GetRegisteredFlows()
        {
            return _flowParser.GetRegisteredFlows();
        }

        public IAsyncEnumerable<string> GetActiveFlowsIds(string flowName)
        {
            return _storage.GetActiveFlowsIds(flowName);
        }        
        
        public IAsyncEnumerable<string> GetAllWaitingFlowsIds()
        {
            return _storage.GetAllWaitingFlowsIds();
        }

        public async Task<IFlowContext> GetFlowRunExecutionContext(string refId)
        {
            var context = await _storage.GetProcessExecutionContext(refId);
            if (context != null)
            {
                context.Params = await UpdateParameters(context.Params);
            }
            return context;
        }


        public async Task<IFlowContext> ExecuteFlow(string flowType, string refId, FlowParamsGeneric parameters, string operationName = null, bool noStorage = false)
        {
            try
            {
                parameters = await UpdateParameters(parameters);

                _fluentFlowRunEngine.UpdateOperation(operationName);

                if (noStorage && string.IsNullOrEmpty(refId))
                {
                    return await _fluentFlowRunEngine.ExecuteFlowNoStorage(flowType, parameters);
                }
                else
                {
                    return await _fluentFlowRunEngine.ExecuteFlow(flowType, refId, parameters);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ExecuteFlow '{flowType}' failed");
                throw;
            }
        }

        private async Task<FlowParamsGeneric> UpdateParameters(FlowParamsGeneric parameters)
        {
            if (parameters?.DynamicInput != null && parameters?.DynamicInput.ContainsKey(PlatformConstants.BaseUri) != true)
            {
                // TODO: find a way to keep parameters immutable!
                parameters[PlatformConstants.BaseUri] = await _clientBrowserService.GetWindowOrigin();
            }
            return parameters;
        }

        public async Task WarmUp()
        {
        }

        public async Task<IUserViewModel> ExecuteSilentFlowForm(string flowType, FlowParamsGeneric flowParams)
        {
            var context = await _fluentFlowRunEngine.ExecuteFlowNoStorage(flowType, await UpdateParameters(flowParams));

            if (context.ExecutionResult.IsFormTask)
            {
                var formTaskDetails = new FlowRunUserViewDetails
                {
                    UserViewName = context.ExecutionResult.FormId,
                    UserViewCallbackTaskName = context.ExecutionResult.CallbackTaskId
                };

                // retrieve data
                //var _queryOptions = queryOptions ?? GetDefaultQueryOptions();
                //UserViewModelPageResult executionResult;

                //using (var wt = new WatchTracer(_logger, "ExecuteFlowTask - read data", _logStreamer))
                //{
                //    executionResult = await _fluentFlowRunEngine.ExecuteFlowTask(flowType, context, formTaskDetails.UserViewCallbackTaskName,
                //        _queryOptions, null);
                //}

                var result = TypeHelper.GetGenericInstance<IUserViewModel>(typeof(UserViewModel<>), context.Model);
                result.RefId = context.RefId;
                result.AccessInfo = new UserViewAccessInformation { AdminUser = context.AdminUser, AssignedUser = context.AssignedUser, AssignedTeam = context.AssignedTeam };
                result.QueryOptions = GetDefaultQueryOptions();

                using (var wt = new WatchTracer(_logger, "Parse Form and ResolveData", _logStreamer))
                {
                    result.SetModel(context.Model, context.Params);
                    //var formType = _flowParser.GetTypeByName(context.ExecutionResult.FormId);
                    //result.UserViewDetails = _formDefinitionParser.Parse(formType);
                    result.UserViewDetails = await GetFormDetails(context.ExecutionResult.FormId);
                }

                _customConfigProvider.DecorateForm(result.UserViewDetails);
                return result;
            }

            _logStreamer.TrackException(new InvalidStateException($"Flow with name={flowType} is not in FormTask state"));
            throw new InvalidStateException($"Flow with name={flowType} is not in FormTask state");
        }

        public async Task<IFlowContext> SubmitSilentFlowForm(string flowType, IFlowModel model, string actionBinding = null, string operationName = null, FlowParamsGeneric flowParams = null)
        {
            flowParams = flowParams ?? new FlowParamsGeneric();
            flowParams.DynamicInput[FlowConstants.SecondPass] = "1";
            
            // UpdateParameters throws exception NavigatinManager has not been initialized
            //var context = await _fluentFlowRunEngine.ExecuteFlowNoStorage(flowType, await UpdateParameters(flowParams));
            var context = await _fluentFlowRunEngine.ExecuteFlowNoStorage(flowType, flowParams);
            
            var refId = context.RefId;
            context.CallStack.Add(context.CurrentTask);
            context.Model = model;
            var result = context.ExecutionResult;
            result.FormState = FormTaskStateEnum.Submitted;
            result.FormLastAction = actionBinding;

            var resultContext = await _fluentFlowRunEngine.ContinueFlowNoStorage(context, operationName, flowParams);
            return resultContext;
        }

        public async Task<IFlowContext> ExecuteClientKeptContextFlow(ClientKeptContext ctx, IFlowModel model, FlowParamsGeneric parameters)
        {
            try
            {
                parameters = await UpdateParameters(parameters);

                _fluentFlowRunEngine.UpdateOperation(parameters.OperationName);

                if (ctx.IsEmpty())
                {
                    return await _fluentFlowRunEngine.ExecuteFlowNoStorage(ctx.FlowName, parameters);
                }
                else
                {
                    return await _fluentFlowRunEngine.ContinueFlowNoStorage(ctx, model, parameters);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ExecuteFlow '{ctx.FlowName}' failed");
                throw;
            }
        }

        public async Task<IFlowContext> SubmitClientKeptContextFlowForm(ClientKeptContext ctx, IFlowModel model, FlowParamsGeneric parameters, string actionBinding)
        {
            var context = new FlowContext(ctx, model);
            context.CallStack.Add(context.CurrentTask);
            context.Model = model;
            var result = context.ExecutionResult;
            result.FormState = FormTaskStateEnum.Submitted;
            result.FormLastAction = actionBinding;

            var resultContext = await _fluentFlowRunEngine.ContinueFlowNoStorage(context, parameters.OperationName, parameters);
            return resultContext;
        }

        // ToDo: add support to skip wasm inline flows where form.FormType is null
        public void ValidateFlow(Type flowType)
        {
            var flowParameters = TypeHelper.GetConstructorParameters(_serviceProvider, flowType);
            var flow = Activator.CreateInstance(flowType, flowParameters) as IFluentFlow;
            var modelType = flow.GetModel().GetType();

            if (flow == null)
            {
                throw new FlowCreateException($"Cannot create flow {flowType} for validation");
            }

            flow.Parse();

            var forms = flow.Tasks.Where(t => t.Type == TaskDefTypes.Form).ToList();

            foreach (var form in forms)
            {
                var formDetails = _formDefinitionParser.Parse(form.FormType);

                // ToDo: currently we only support forms that have a generic base type with a model argument, like FormEditBase<Model>
                var baseType = form.FormType.BaseType;
                var formModelType = baseType.GetGenericArguments()[0];

                if (formModelType != modelType)
                {
                    throw new BlazorFormsValidationException($"Type mismatch: Flow {flowType.Name} defined with model {modelType} " +
                        $"but referenced form {form.FormType.Name} has a wrong model {formModelType}");
                }

                var rules = formDetails.Fields.SelectMany(f => f.FlowRules);

                foreach(var rule in rules)
                {
                    var ruleType = _flowParser.GetTypeByName(rule.FormRuleType);
                    var baseRuleType = ruleType.BaseType;
                    var ruleModelType = baseRuleType.GetGenericArguments()[0];

                    if (ruleModelType != modelType)
                    {
                        throw new BlazorFormsValidationException($"Type mismatch: Flow {flowType.Name} defined with model {modelType} " +
                            $"but referenced form {form.FormType.Name} has a rule {ruleType} with a wrong model {ruleModelType}");
                    }
                }
            }
        }
    }
}
