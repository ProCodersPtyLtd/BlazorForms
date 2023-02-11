using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using BlazorForms.Admin.BusinessObjects.Interfaces;
using BlazorForms.Admin.BusinessObjects.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Admin.BusinessObjects.Providers
{
    public class FlowDataProvider : IFlowDataProvider
    {
        private readonly IFluentFlowRunEngine _flowEngine;
        private readonly IStateFlowRunEngine _stateFlowEngine;
        private readonly IServiceProvider _serviceProvider;
        private readonly IFlowRunStorage _flowRunStorage;

        public FlowDataProvider(IServiceProvider serviceProvider, IStateFlowRunEngine stateFlowEngine, IFluentFlowRunEngine flowEngine, 
            IFlowRunStorage flowRunStorage)
        {
            _flowEngine = flowEngine;
            _stateFlowEngine = stateFlowEngine;
            _serviceProvider = serviceProvider;
            _flowRunStorage = flowRunStorage;
        }

        public async Task<List<FlowDataDetails>> GetRegisteredFlows(FlowDataOptions options)
        {
            var result = new List<FlowDataDetails>();
            var flows = _flowEngine.GetAllFlowTypes();
            var showNamespaces = options.ShowNamespaces;

            foreach (var flowType in flows)
            {
                var flowParameters = TypeHelper.GetConstructorParameters(_serviceProvider, flowType);
                var flow = Activator.CreateInstance(flowType, flowParameters) as IFluentFlow;
                flow.Parse();
                var modelType = flow.GetModelType();

                var item = new FlowDataDetails
                {
                    FlowInterface = "IFluentFlow",
                    RefId = flowType.FullName.Replace(".", "-"),
                    FlowType = showNamespaces == true ? flowType.FullName : flowType.Name,
                    ModelType = showNamespaces == true ? modelType.FullName : modelType.Name,
                    TaskCount = flow.Tasks.Count,
                };

                result.Add(item);
            }

            var stateFlows = _stateFlowEngine.GetAllFlowTypes();

            foreach (var flowType in stateFlows)
            {
                var flowParameters = TypeHelper.GetConstructorParameters(_serviceProvider, flowType);
                var flow = Activator.CreateInstance(flowType, flowParameters) as IStateFlow;
                flow.Parse();
                var modelType = flow.GetModelType();

                var item = new FlowDataDetails
                {
                    FlowInterface = "IStateFlow",
                    RefId = flowType.FullName.Replace(".", "-"),
                    FlowType = showNamespaces == true ? flowType.FullName : flowType.Name,
                    ModelType = showNamespaces == true ? modelType.FullName : modelType.Name,
                    TaskCount = flow.States.Count,
                };

                result.Add(item);
            }


            return result;
        }

        public async Task<List<FlowDataDetails>> GetStoredFlows(QueryOptions queryOptions, FlowDataOptions options)
        {
            var result = new List<FlowDataDetails>();
            var flowModelQueryOptions = new FlowModelsQueryOptions { QueryOptions = queryOptions };
            var showNamespaces = options.ShowNamespaces;

            var registeredFlows = await GetRegisteredFlows(options);
            var dict = registeredFlows.ToDictionary(f => f.FlowType, f => f);

            var storedFlows = await _flowRunStorage.GetFlowContexts(flowModelQueryOptions);

            foreach (var flow in storedFlows)
            {
                FlowDataDetails item;

                if (dict.ContainsKey(flow.FlowName))
                {
                    item = dict[flow.FlowName].GetCopy();
                }
                else
                {
                    item = new FlowDataDetails
                    {
                        RefId = flow.RefId,
                        FlowType = flow.FlowName,
                        ModelType = flow.ModelType,
                    };
                }

                item.RefId = flow.RefId;
                item.ModelJson = flow.ModelJson;
                item.FlowState = flow.ExecutionResult?.FlowState.ToString();
                item.ResultState = flow.ExecutionResult?.ResultState.ToString();
                item.CreatedDate = flow.ExecutionResult?.CreatedDate;
                item.ChangedDate = flow.ExecutionResult?.ChangedDate;
                item.FinishedDate = flow.ExecutionResult?.FinishedDate;
                result.Add(item);
            }

            return result;
        }
    }
}
