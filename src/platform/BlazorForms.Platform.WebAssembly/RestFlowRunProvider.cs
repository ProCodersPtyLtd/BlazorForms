using Newtonsoft.Json;
using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using BlazorForms.Platform;
using BlazorForms.Platform.Definitions.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;
using SJ = System.Text.Json;

namespace BlazorForms.Platform
{
    public class RestFlowRunProvider : IFlowRunProvider
    {
        private readonly HttpClient _http;
        private readonly IKnownTypesBinder _knownTypesBinder;

        public RestFlowRunProvider(HttpClient http, IKnownTypesBinder knownTypesBinder)
        {
            _http = http;
            _knownTypesBinder = knownTypesBinder;

        }

        protected async Task<T> ReadResponse<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error code {response.StatusCode.AsInt()} with reason '{response.ReasonPhrase}' returned from '{response.RequestMessage.RequestUri}'");
            }

            var content = await response.Content.ReadAsStringAsync();
            // Console.WriteLine(content);
            var result = SJ.JsonSerializer.Deserialize<T>(content);
            return result;
        }

        protected void CheckResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error code {response.StatusCode.AsInt()} with reason '{response.ReasonPhrase}' returned from '{response.RequestMessage.RequestUri}'");
            }
        }

        protected async Task<string> ReadContent(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error code {response.StatusCode.AsInt()} with reason '{response.ReasonPhrase}' returned from '{response.RequestMessage.RequestUri}'");
            }

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        protected ByteArrayContent GetJsonContent(object data)
        {
            var myContent = SJ.JsonSerializer.Serialize(data, typeof(object),
                new SJ.JsonSerializerOptions { IgnoreReadOnlyProperties = true, PropertyNameCaseInsensitive = false, PropertyNamingPolicy = null });

            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return byteContent;
        }

        public async Task<bool> CheckFormAccess(FormAccessDetails access, IFlowContext context)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var ctxWrapper = FlowContextWrapper.CreateInstance(context);
            var byteContent = GetJsonContent(new CheckFormAccessDataUntyped { Access = access, Context = ctxWrapper, ModelFullName = context.Model.GetType().FullName });
            var response = await _http.PostAsync($"/api/flow/checkFormAccess", byteContent);
            return await ReadResponse<bool>(response);
        }

        public async Task<bool> CheckFormAccess(FormAccessDetails access, UserViewAccessInformation accessInfo, IFlowModel model, FlowParamsGeneric flowParams)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var modelWrapper = FlowModelWrapper.CreateInstance(model);
            
            var byteContent = GetJsonContent(new CheckFormAccessModelDataUntyped 
            { 
                Access = access, 
                Model = modelWrapper, 
                ModelFullName = model.GetType().FullName,
                AccessInfo = accessInfo
            });

            var response = await _http.PostAsync($"/api/flow/checkFormAccessModel", byteContent);
            return await ReadResponse<bool>(response);
        }

        public async Task<FormDetails> GetFormDetails(string formName)
        {
            FormDetails result = null;
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await _http.GetAsync($"/api/flow/getFormDetails/{formName}");

            var watch = System.Diagnostics.Stopwatch.StartNew();

            result = await ReadResponse<FormDetails>(response);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"[GetFormDetails] Elapsed {elapsedMs} ms");

            return result;
        }

        public I Deserialize<I>(string json, Type generic)
            where I: class
        {
            var obj = SJ.JsonSerializer.Deserialize<JsonModelWrapper>(json);
            var modelTypeName = obj.ModelFullName;
            var mt = modelTypeName == null ? typeof(object) : _knownTypesBinder.KnownTypesDict[modelTypeName];
            var targetType = generic.MakeGenericType(new Type[] { mt });

            var result = SJ.JsonSerializer.Deserialize(json, targetType) as I;
            return result;
        }

        public async Task<IUserViewModel> GetListItemFlowUserView(string flowType, FlowParamsGeneric parameters)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var byteContent = GetJsonContent(parameters);
            var response = await _http.PostAsync($"/api/flow/listItemFlowUserView/{flowType}", byteContent);
            var content = await ReadContent(response);
            var view = Deserialize<IUserViewModel>(content, typeof(UserViewModel<>));
            return view;
        }

        #region not required for Client Blazor

        // not required for Client Blazor
        public IEnumerable<string> GetRegisteredFlows()
        {
            throw new NotImplementedException();
        }

        // not required for Client Blazor
        public Task WarmUp()
        {
            throw new NotImplementedException();
        }

        public async Task FinishFlow(int flowRunId, IFlowModel model, string actionBinding = null, string operationName = null)
        {
            throw new NotImplementedException();
        }
        #endregion

        public async Task<IFlowContext> SubmitListItemForm(string refId, IFlowModel model, string operationName = null)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var modelWrapper = FlowModelWrapper.CreateInstance(model);
            var byteContent = GetJsonContent(new ModelAndParametersDataUntyped { Params = null, Model = modelWrapper, ModelFullName = model.GetType().FullName });
            var response = await _http.PostAsync($"/api/flow/submitListItemForm/{refId}/'{operationName}'", byteContent);
            var content = await ReadContent(response);
            var fullContext = FlowContextWrapper.DeserializeContext(content, _knownTypesBinder);
            return fullContext;

        }

        public async Task SaveForm(string refId, IFlowModel model, string actionBinding = null, string operationName = null)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var modelWrapper = FlowModelWrapper.CreateInstance(model);
            var byteContent = GetJsonContent(modelWrapper);
            var response = await _http.PostAsync($"/api/flow/saveForm/{refId}/'{actionBinding}'/'{operationName}'", byteContent);
            CheckResponse(response);
        }

        // ToDo: do we need this?
        public async Task FinishFlow(string refId, IFlowModel model, string actionBinding = null, string operationName = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IFlowContext> SubmitForm(string refId, IFlowModel model, string actionBinding = null, string operationName = null, FlowParamsGeneric flowParams = null)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var modelWrapper = FlowModelWrapper.CreateInstance(model);
            var byteContent = GetJsonContent(modelWrapper);
            var response = await _http.PostAsync($"/api/flow/submitForm/{refId}/'{actionBinding}'/'{operationName}'", byteContent);
            var content = await ReadContent(response);
            var fullContext = FlowContextWrapper.DeserializeContext(content, _knownTypesBinder);
            return fullContext;
        }

        public async Task<IFlowContext> RejectForm(string refId, IFlowModel model, string actionBinding = null, string operationName = null)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var modelWrapper = FlowModelWrapper.CreateInstance(model);
            var byteContent = GetJsonContent(modelWrapper);
            var response = await _http.PostAsync($"/api/flow/rejectForm/{refId}/'{actionBinding}'/'{operationName}'", byteContent);
            var content = await ReadContent(response);
            var fullContext = FlowContextWrapper.DeserializeContext(content, _knownTypesBinder);
            return fullContext;
        }

        public async Task<IUserViewModel> GetFlowDefaultReadonlyView(string refId)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await _http.GetAsync($"/api/flow/defaultReadonlyView/{refId}");
            var content = await ReadContent(response);
            var view = Deserialize<IUserViewModel>(content, typeof(UserViewModel<>));
            return view;
        }

        // ToDo: do we need this? Case now implemented when flowContext is null
        public async Task<FlowRunUserViewDetails> GetCurrentFlowRunUserView(string refId, IFlowContext flowContext = null)
        {
            var result = new FlowRunUserViewDetails();

            var context = flowContext ?? await GetFlowRunExecutionContext(refId);
 
            if (context.ExecutionResult.IsFormTask)
            {
                result.UserViewName = context.ExecutionResult.FormId;
                result.UserViewCallbackTaskName = context.ExecutionResult.CallbackTaskId;
                return result;
            }

            throw new InvalidStateException($"Flow with refId {refId} is not in FormTask state");
        }

        public async Task<IFlowContext> GetFlowRunExecutionContext(string refId)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await _http.GetAsync($"/api/flow/getFlowContext/{refId}");
            var content = await ReadContent(response);
            var fullContext = FlowContextWrapper.DeserializeContext(content, _knownTypesBinder);
            return fullContext;
        }

        public async Task<IFlowModel> GetLastModel(string refId)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await _http.GetAsync($"/api/flow/getLastModel/{refId}");
            var content = await ReadContent(response);
            var model = FlowModelWrapper.Deserialize(content, _knownTypesBinder);
            return model;
        }

        public IAsyncEnumerable<string> GetActiveFlowsIds(string flowName)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            async IAsyncEnumerable<string> StreamData()
            {
                using HttpResponseMessage response = await _http.GetAsync($"/api/flow/activeFlows/{flowName}", HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<IAsyncEnumerable<string>>().ConfigureAwait(false);

                await foreach (var result in data)
                {
                    yield return result;
                }
            }

            return StreamData();
        }    
        
        public IAsyncEnumerable<string> GetAllWaitingFlowsIds()
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            async IAsyncEnumerable<string> StreamData()
            {
                using HttpResponseMessage response = await _http.GetAsync($"/api/flow/allWaitingFlowsIds", HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<IAsyncEnumerable<string>>().ConfigureAwait(false);

                await foreach (var result in data)
                {
                    yield return result;
                }
            }

            return StreamData();
        }

        public async Task<IFlowContext> ExecuteFlow(string flowType, string refId, FlowParamsGeneric parameters, string operationName = null, bool noStorage = false)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var byteContent = GetJsonContent(parameters as FlowParamsGeneric);
            var response = await _http.PostAsync($"/api/flow/execute/{flowType}/'{refId}'/'{operationName}'/{noStorage}", byteContent);
            var content = await ReadContent(response);
            var fullContext = FlowContextWrapper.DeserializeContext(content, _knownTypesBinder);
            return fullContext;
        }

        public async Task<IUserViewModel> GetListFlowUserView(string flowType, FlowParamsGeneric parameters, QueryOptions queryOptions)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var wrapper = new QueryOptionsAndParams { QueryOptions = queryOptions, Params = parameters };
            var byteContent = GetJsonContent(wrapper);
            var response = await _http.PostAsync($"/api/flow/listFlowUserView/{flowType}/{queryOptions.PageIndex}/{queryOptions.PageSize}", byteContent);
            var content = await ReadContent(response);
            var view = Deserialize<IUserViewModel>(content, typeof(UserViewModel<>));
            return view;
        }

        #region Silent Flow Forms

        // ToDo: add test
        public async Task<IUserViewModel> ExecuteSilentFlowForm(string flowType, FlowParamsGeneric flowParams)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var byteContent = GetJsonContent(flowParams);
            var response = await _http.PostAsync($"/api/flow/executeSilentFlowForm/{flowType}", byteContent);
            var content = await ReadContent(response);
            var view = Deserialize<IUserViewModel>(content, typeof(UserViewModel<>));
            return view;
        }

        public async Task<IFlowContext> SubmitSilentFlowForm(string flowType, IFlowModel model, string actionBinding = null, string operationName = null, FlowParamsGeneric flowParams = null)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var modelWrapper = FlowModelWrapper.CreateInstance(model);
            var byteContent = GetJsonContent(modelWrapper);
            var response = await _http.PostAsync($"/api/flow/submitSilentFlowForm/{flowType}/'{actionBinding}'/'{operationName}'", byteContent);
            var content = await ReadContent(response);
            var fullContext = FlowContextWrapper.DeserializeContext(content, _knownTypesBinder);
            return fullContext;
        }

        #endregion

        #region Rules

        // ToDo: we need to supply fields collection here for fluent forms
        public async Task<RuleEngineExecutionResult> ExecuteFormLoadRules(RuleExecutionRequest ruleRequest, IFlowModel model)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var modelWrapper = FlowModelWrapper.CreateInstance(model);
            var byteContent = GetJsonContent(new ModelAndRuleExecutionRequestUntyped { Request = ruleRequest, Model = modelWrapper, ModelFullName = model.GetType().FullName });
            var response = await _http.PostAsync($"/api/flow/loadRules/{model.GetType().FullName}", byteContent);
            var content = await ReadContent(response);
            var fullResult = RuleEngineExecutionResultWrapper.Deserialize(content, _knownTypesBinder);
            return fullResult;
        }

        public async Task<RuleEngineExecutionResult> TriggerRule(RuleExecutionRequest ruleRequest, IFlowModel model)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var modelWrapper = FlowModelWrapper.CreateInstance(model);
            var byteContent = GetJsonContent(new ModelAndRuleExecutionRequestUntyped { Request = ruleRequest, Model = modelWrapper, ModelFullName = model.GetType().FullName });
            var response = await _http.PostAsync($"/api/flow/triggerRules/{model.GetType().FullName}'", byteContent);
            var content = await ReadContent(response);
            var fullResult = RuleEngineExecutionResultWrapper.Deserialize(content, _knownTypesBinder);
            return fullResult;
        }

        // ToDo: flowParams not sent
        public async Task<RuleEngineExecutionResult> TriggerRule(string formTypeFullName, IFlowModel model, FlowParamsGeneric flowParams, 
            FieldDisplayDetails[] displayProperties, FormRuleTriggers? trigger, string ruleCode, FieldBinding fieldBinding, int rowIndex = 0)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var modelWrapper = FlowModelWrapper.CreateInstance(model);
            var byteContent = GetJsonContent(new ModelAndFieldDisplayDetailsUntyped { Details = displayProperties, Model = modelWrapper, Binding = fieldBinding, ModelFullName = model.GetType().FullName });
            var response = await _http.PostAsync($"/api/flow/triggerRule/{formTypeFullName}/'{(int?)trigger}'/'{ruleCode}'/'{rowIndex}'", byteContent);
            var content = await ReadContent(response);
            var fullResult = RuleEngineExecutionResultWrapper.Deserialize(content, _knownTypesBinder);
            return fullResult;
        }

        #endregion

        public async Task<IFlowContext> ExecuteClientKeptContextFlow(ClientKeptContext ctx, IFlowModel model, FlowParamsGeneric parameters)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var nullableModel = model ?? new NullFlowModel();
            var modelWrapper = FlowModelWrapper.CreateInstance(nullableModel);
            
            var byteContent = GetJsonContent(new ModelAndClientKeptContextRequestUntyped 
            { 
                Ctx = ctx, 
                Parameters = parameters,
                Model = modelWrapper, 
                ModelFullName = nullableModel.GetType().FullName 
            });
            
            var response = await _http.PostAsync($"/api/flow/executeClientKeptContextFlow/{nullableModel.GetType().FullName}", byteContent);
            var content = await ReadContent(response);
            var fullResult = FlowContextWrapper.DeserializeContext(content, _knownTypesBinder);
            return fullResult;
        }

        public async Task<IFlowContext> SubmitClientKeptContextFlowForm(ClientKeptContext ctx, IFlowModel model, FlowParamsGeneric parameters, string actionBinding)
        {
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var modelWrapper = FlowModelWrapper.CreateInstance(model);

            var byteContent = GetJsonContent(new ModelAndClientKeptContextRequestUntyped
            {
                Ctx = ctx,
                Parameters = parameters,
                Model = modelWrapper,
                ModelFullName = model.GetType().FullName
            });

            var response = await _http.PostAsync($"/api/flow/submitClientKeptContextFlowForm/'{actionBinding}'", byteContent);
            var content = await ReadContent(response);
            var fullResult = FlowContextWrapper.DeserializeContext(content, _knownTypesBinder);
            return fullResult;
        }

        public void ValidateFlow(Type flowType)
        {
            throw new NotImplementedException("Flow Validations done only on server side when app.BlazorFormsRun() is executed");
        }
    }
}
