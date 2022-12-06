using Microsoft.AspNetCore.Mvc;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Threading.Tasks;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.FlowRules;
using SJ = System.Text.Json;
using System.Text;
using BlazorForms.Shared.Extensions;
using BlazorForms.Platform.Definitions.Wrappers;

namespace BlazorForms.Platform.Api.Controllers
{
    [ApiController]
    [Route("api/flow")]
    public class FlowFormsApiController : ControllerBase
    {
        private readonly IFlowRunProvider _flowRunProvider;
        private readonly IKnownTypesBinder _knownTypesBinder;

        public FlowFormsApiController(IFlowRunProvider processFlowProvider, IKnownTypesBinder knownTypesBinder)
        {
            _flowRunProvider = processFlowProvider;
            _knownTypesBinder = knownTypesBinder;
            Console.WriteLine("Completed FlowFormsController Constructor");
        }

        [HttpPost("execute/{flowType}/{refId}/{operationName}/{noStorage}")]
        public async Task<object> ExecuteFlow(string flowType, string refId, string operationName, bool noStorage, [FromBody] FlowParamsGeneric flowParams)
        {
            Console.WriteLine("Executing FlowFormsController ExecuteFlow");
            var result = await _flowRunProvider.ExecuteFlow(flowType, refId?.RemoveQuotesNull(), flowParams, operationName?.RemoveQuotes(), noStorage);
            var dto = FlowContextWrapper.CreateInstance(result);
            Console.WriteLine("Completed FlowFormsController ExecuteFlow");
            return dto;
        }

        [HttpGet("getFlowContext/{refId}")]
        public async Task<object> GetFlowRunExecutionContext(string refId)
        {
            var result = await _flowRunProvider.GetFlowRunExecutionContext(refId);
            var dto = FlowContextWrapper.CreateInstance(result);
            return dto;
        }

        [HttpGet("getFormDetails/{flowName}")]
        public async Task<FormDetails> GetFormDetails(string flowName)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var result = await _flowRunProvider.GetFormDetails(flowName);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"[getFormDetails/] Elapsed {elapsedMs} ms");

            return result;
        }

        [HttpGet("getLastModel/{refId}")]
        public async Task<object> GetLastModel(string refId)
        {
            var result = await _flowRunProvider.GetLastModel(refId);
            var dto = FlowModelWrapper.CreateInstance(result);
            return dto;
        }

        [HttpPost("checkFormAccess")]
        public async Task<bool> CheckFormAccess()
        {
            using (var reader = new StreamReader(
                Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 4096,
                leaveOpen: true))
            {
                var json = await reader.ReadToEndAsync();
                //Request.Body.Position = 0;

                var obj = SJ.JsonSerializer.Deserialize<JsonModelWrapper>(json);
                var modelTypeName = obj.ModelFullName;
                var mt = _knownTypesBinder.KnownTypesDict[modelTypeName];
                var targetType = typeof(CheckFormAccessData<>).MakeGenericType(new Type[] { mt });
                var wrapper = SJ.JsonSerializer.Deserialize(json, targetType) as CheckFormAccessData;

                var data = wrapper.ContextUntyped as FlowContextWrapper;
                var ctx = new FlowContext(data, data.ModelUntyped as IFlowModel);
                var result = await _flowRunProvider.CheckFormAccess(wrapper.Access, ctx);
                return result;
            }
        }

        [HttpPost("checkFormAccessModel")]
        public async Task<bool> CheckFormAccessModel()
        {
            using (var reader = new StreamReader(
                Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 4096,
                leaveOpen: true))
            {
                var json = await reader.ReadToEndAsync();
                //Request.Body.Position = 0;

                var obj = SJ.JsonSerializer.Deserialize<JsonModelWrapper>(json);
                var modelTypeName = obj.ModelFullName;
                var mt = _knownTypesBinder.KnownTypesDict[modelTypeName];
                var targetType = typeof(CheckFormAccessModelData<>).MakeGenericType(new Type[] { mt });
                var wrapper = SJ.JsonSerializer.Deserialize(json, targetType) as CheckFormAccessModelData;

                var modelWrapper = wrapper.ModelUntyped as FlowModelWrapper;
                var model = modelWrapper.ModelUntyped as IFlowModel;
                var result = await _flowRunProvider.CheckFormAccess(wrapper.Access, wrapper.AccessInfo, model, wrapper.FlowParams);
                return result;
            }


        }

        [HttpGet("activeFlows/{flowName}")]
        public IAsyncEnumerable<string> GetActiveFlowsIds(string flowName)
        {
            var result = _flowRunProvider.GetActiveFlowsIds(flowName);
            return result;
        }
        
        [HttpGet("allWaitingFlowsIds")]
        public IAsyncEnumerable<string> GetAllWaitingFlowsIds()
        {
            var result = _flowRunProvider.GetAllWaitingFlowsIds();
            return result;
        }

        [HttpPost("listFlowUserView/{flowType}/{pageIndex}/{pageSize}")]
        public async Task<object> GetListFlowUserView(string flowType, int pageIndex, int pageSize, [FromBody] QueryOptionsAndParams data)
        {
            var result = await _flowRunProvider.GetListFlowUserView(flowType, data.Params, 
                data.QueryOptions ?? new QueryOptions { PageIndex = pageIndex, PageSize = pageSize });

            return result;
        }

        [HttpGet("defaultReadonlyView/{refId}")]
        public async Task<object> GetFlowDefaultReadonlyView(string refId)
        {
            var result = await _flowRunProvider.GetFlowDefaultReadonlyView(refId);
            return result;
        }

        [HttpPost("listItemFlowUserView/{flowType}")]
        public async Task<object> GetListItemFlowUserView(string flowType, [FromBody] FlowParamsGeneric parameters)
        {
            var result = await _flowRunProvider.GetListItemFlowUserView(flowType, parameters);
            return result;
        }

        [HttpPost("rejectForm/{refId}/{actionBinding}/{operationName}")]
        public async Task<object> RejectForm(string refId, string actionBinding, string operationName)
        {
            using (var reader = new StreamReader(
                Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 4096,
                leaveOpen: true))
            {
                actionBinding = actionBinding.RemoveQuotes();
                operationName = operationName.RemoveQuotes();

                var json = await reader.ReadToEndAsync();
                var model = FlowModelWrapper.Deserialize(json, _knownTypesBinder);

                var ctx = await _flowRunProvider.RejectForm(refId, model, actionBinding, operationName);
                var dto = FlowContextWrapper.CreateInstance(ctx);
                return dto;
            }
        }

        [HttpPost("saveForm/{refId}/{actionBinding}/{operationName}")]
        public async Task SaveForm(string refId, string actionBinding, string operationName)
        {
            using (var reader = new StreamReader(
                Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 4096,
                leaveOpen: true))
            {
                actionBinding = actionBinding.RemoveQuotes();
                operationName = operationName.RemoveQuotes();

                var json = await reader.ReadToEndAsync();
                var model = FlowModelWrapper.Deserialize(json, _knownTypesBinder);

                await _flowRunProvider.SaveForm(refId, model, actionBinding, operationName);
            }
        }

        [HttpPost("submitForm/{refId}/{actionBinding}/{operationName}")]
        public async Task<object> SubmitForm(string refId, string actionBinding, string operationName) // TODO YB: Why not [FromBody]JsonModelWrapper modelWrapper?
        {
            using (var reader = new StreamReader(
                Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 4096,
                leaveOpen: true))
            {
                actionBinding = actionBinding.RemoveQuotes();
                operationName = operationName.RemoveQuotes();

                var json = await reader.ReadToEndAsync();
                var model = FlowModelWrapper.Deserialize(json, _knownTypesBinder);

                var ctx = await _flowRunProvider.SubmitForm(refId, model, actionBinding, operationName);
                var dto = FlowContextWrapper.CreateInstance(ctx);
                return dto;
            }
        }

        [HttpPost("submitListItemForm/{refId}/{operationName}")]
        public async Task<object> SubmitListItemForm(string refId, string operationName)
        {
            using (var reader = new StreamReader(
                Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 4096,
                leaveOpen: true))
            {
                operationName = operationName.RemoveQuotes();
                var json = await reader.ReadToEndAsync();

                var obj = SJ.JsonSerializer.Deserialize<JsonModelWrapper>(json);
                var modelTypeName = obj.ModelFullName;
                var mt = _knownTypesBinder.KnownTypesDict[modelTypeName];
                var targetType = typeof(ModelAndParameters<>).MakeGenericType(new Type[] { mt });
                var wrapper = SJ.JsonSerializer.Deserialize(json, targetType) as ModelAndParameters;

                var model = wrapper.ModelUntyped as IFlowModel;
                var result = await _flowRunProvider.SubmitListItemForm(refId, model, operationName);
                return result;
            }
            throw new NotImplementedException("ToDo: WASM support will be added later");
        }

        // ToDo: flowParams not sent and not implemented in REST
        [HttpPost("triggerRule/{formType}/{trigger}/{ruleCode}/{rowIndex}")]
        public async Task<object> TriggerRule(string formType, string trigger, string ruleCode, string rowIndex)
        {
            using (var reader = new StreamReader(
                Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 4096,
                leaveOpen: true))
            {
                ruleCode = ruleCode.RemoveQuotes();
                int x;
                FormRuleTriggers? triggerParam = null;
                int rowIndexParam = 0;
                int.TryParse(rowIndex.RemoveQuotes(), out rowIndexParam);

                if (int.TryParse(trigger.RemoveQuotes(), out x))
                {
                    triggerParam = (FormRuleTriggers)x;
                }

                var json = await reader.ReadToEndAsync();

                var obj = SJ.JsonSerializer.Deserialize<JsonModelWrapper>(json);
                var modelTypeName = obj.ModelFullName;
                var mt = _knownTypesBinder.KnownTypesDict[modelTypeName];
                var targetType = typeof(ModelAndFieldDisplayDetails<>).MakeGenericType(new Type[] { mt });
                var wrapper = SJ.JsonSerializer.Deserialize(json, targetType) as ModelAndFieldDisplayDetails;

                var model = wrapper.ModelUntyped as IFlowModel;
                var result = await _flowRunProvider.TriggerRule(formType, model, null, wrapper.Details, triggerParam, ruleCode, wrapper.Binding, rowIndexParam);
                var dto = RuleEngineExecutionResultWrapper.CreateInstance(result);
                return dto;
            }
        }

        [HttpPost("loadRules/{s}")]
        public async Task<object> ExecuteFormLoadRules(string s)
        {
            using (var reader = new StreamReader(
                Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 4096,
                leaveOpen: true))
            {
                var json = await reader.ReadToEndAsync();
                var obj = SJ.JsonSerializer.Deserialize<JsonModelWrapper>(json);
                var modelTypeName = obj.ModelFullName;
                var mt = _knownTypesBinder.KnownTypesDict[modelTypeName];
                var targetType = typeof(ModelAndRuleExecutionRequest<>).MakeGenericType(new Type[] { mt });
                var wrapper = SJ.JsonSerializer.Deserialize(json, targetType) as ModelAndRuleExecutionRequest;
                var model = wrapper.ModelUntyped as IFlowModel;

                var result = await _flowRunProvider.ExecuteFormLoadRules(wrapper.Request, model);
                var dto = RuleEngineExecutionResultWrapper.CreateInstance(result);
                return dto;
            }
        }

        [HttpPost("triggerRules/{s}")]
        public async Task<object> TriggerRules(string s)
        {
            using (var reader = new StreamReader(
                Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 4096,
                leaveOpen: true))
            {
                var json = await reader.ReadToEndAsync();
                var obj = SJ.JsonSerializer.Deserialize<JsonModelWrapper>(json);
                var modelTypeName = obj.ModelFullName;
                var mt = _knownTypesBinder.KnownTypesDict[modelTypeName];
                var targetType = typeof(ModelAndRuleExecutionRequest<>).MakeGenericType(new Type[] { mt });
                var wrapper = SJ.JsonSerializer.Deserialize(json, targetType) as ModelAndRuleExecutionRequest;
                var model = wrapper.ModelUntyped as IFlowModel;

                var result = await _flowRunProvider.TriggerRule(wrapper.Request, model);
                var dto = RuleEngineExecutionResultWrapper.CreateInstance(result);
                return dto;
            }
        }

        [HttpPost("executeSilentFlowForm/{flowType}")]
        public async Task<object> ExecuteSilentFlowForm(string flowType, [FromBody] FlowParamsGeneric parameters)
        {
            var result = await _flowRunProvider.ExecuteSilentFlowForm(flowType, parameters);
            //var json = SJ.JsonSerializer.Serialize(result, typeof(object), new SJ.JsonSerializerOptions { IgnoreReadOnlyProperties = true });
            return result;
        }

        [HttpPost("submitSilentFlowForm/{flowType}/{actionBinding}/{operationName}")]
        public async Task<object> SubmitSilentFlowForm(string flowType, string actionBinding, string operationName)
        {
            using (var reader = new StreamReader(
                Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 4096,
                leaveOpen: true))
            {
                actionBinding = actionBinding.RemoveQuotes();
                operationName = operationName.RemoveQuotes();

                var json = await reader.ReadToEndAsync();
                var model = FlowModelWrapper.Deserialize(json, _knownTypesBinder);

                var ctx = await _flowRunProvider.SubmitSilentFlowForm(flowType, model, actionBinding, operationName);
                var dto = FlowContextWrapper.CreateInstance(ctx);
                return dto;
            }
        }

        [HttpPost("executeClientKeptContextFlow/{modelName}")]
        public async Task<object> ExecuteClientKeptContextFlow(string modelName)
        {
            using (var reader = new StreamReader(
                Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 4096,
                leaveOpen: true))
            {
                Console.WriteLine("Executing FlowFormsController ExecuteClientKeptContextFlow");
                var json = await reader.ReadToEndAsync();
                var obj = SJ.JsonSerializer.Deserialize<JsonModelWrapper>(json);
                var modelTypeName = obj.ModelFullName;
                var mt = _knownTypesBinder.KnownTypesDict[modelTypeName];
                var targetType = typeof(ModelAndClientKeptContextRequest<>).MakeGenericType(new Type[] { mt });
                var wrapper = SJ.JsonSerializer.Deserialize(json, targetType) as ModelAndClientKeptContextRequest;
                var model = wrapper.ModelUntyped as IFlowModel;

                var result = await _flowRunProvider.ExecuteClientKeptContextFlow(wrapper.Ctx, model, wrapper.Parameters);
                var dto = FlowContextWrapper.CreateInstance(result);
                Console.WriteLine("Completed FlowFormsController ExecuteClientKeptContextFlow");
                return dto;
            }
        }

        [HttpPost("submitClientKeptContextFlowForm/{actionBinding}")]
        public async Task<object> SubmitClientKeptContextFlowForm(string actionBinding)
        {
            using (var reader = new StreamReader(
                Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 4096,
                leaveOpen: true))
            {
                Console.WriteLine("Executing FlowFormsController ExecuteClientKeptContextFlow");
                actionBinding = actionBinding.RemoveQuotes();
                var json = await reader.ReadToEndAsync();
                var obj = SJ.JsonSerializer.Deserialize<JsonModelWrapper>(json);
                var modelTypeName = obj.ModelFullName;
                var mt = _knownTypesBinder.KnownTypesDict[modelTypeName];
                var targetType = typeof(ModelAndClientKeptContextRequest<>).MakeGenericType(new Type[] { mt });
                var wrapper = SJ.JsonSerializer.Deserialize(json, targetType) as ModelAndClientKeptContextRequest;
                var model = wrapper.ModelUntyped as IFlowModel;

                var result = await _flowRunProvider.SubmitClientKeptContextFlowForm(wrapper.Ctx, model, wrapper.Parameters, actionBinding);
                var dto = FlowContextWrapper.CreateInstance(result);
                Console.WriteLine("Completed FlowFormsController ExecuteClientKeptContextFlow");
                return dto;
            }
        }

    }
}
