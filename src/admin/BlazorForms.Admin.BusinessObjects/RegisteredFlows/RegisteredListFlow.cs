using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using BlazorForms.Admin.BusinessObjects.Interfaces;
using BlazorForms.Admin.BusinessObjects.Model;
using BlazorForms.Platform;
using BlazorForms.Admin.BusinessObjects.StorageFlows;

namespace BlazorForms.Admin.BusinessObjects.RegisteredFlows
{
    public class RegisteredListFlow : FluentFlowBase<FlowDetailsModel>
    {
        private readonly IFlowDataProvider _flowDataProvider;
        private readonly IFluentFlowRunEngine _flowEngine;
        private readonly IServiceProvider _serviceProvider;

        private List<Type> _ignoreFlows = new Type[] 
        { 
            typeof(RegisteredListFlow), 
            typeof(RegisteredListItemViewFlow), 
            typeof(StoredListFlow), 
            typeof(ErrorEditFlow),  
        }.ToList();

        public RegisteredListFlow(IFlowDataProvider flowDataProvider, IFluentFlowRunEngine flowEngine, IServiceProvider serviceProvider)
        {
            _flowDataProvider = flowDataProvider;
            _flowEngine = flowEngine;
            _serviceProvider = serviceProvider;
        }

        public override void Define()
        {
            this.ListForm(typeof(RegisteredListForm), ViewDataCallbackTask, true);
        }
        public async Task<FlowDetailsModel> ViewDataCallbackTask(QueryOptions queryOptions)
        {
            var result = new FlowDetailsModel();
            var options = new FlowDataOptions();
            options.ShowNamespaces = Params.GetParam("ShowNamespaces").AsBool();
            var list = await _flowDataProvider.GetRegisteredFlows(options);

            if (options.ShowNamespaces == true)
            {
                list = list.Where(f => !_ignoreFlows.Any(i => i.FullName == f.FlowType)).ToList();
            }
            else 
            {
                list = list.Where(f => !_ignoreFlows.Any(i => i.Name == f.FlowType)).ToList();
            }

            result.Data = list;
            return result;

            //var flows = _flowEngine.GetAllFlowTypes();
            //var showNamespaces = Params.GetParam("ShowNamespaces").AsBool();

            //foreach (var flowType in flows)
            //{
            //    var flowParameters = TypeHelper.GetConstructorParameters(_serviceProvider, flowType);
            //    var flow = Activator.CreateInstance(flowType, flowParameters) as IFluentFlow;
            //    flow.Parse();
            //    var modelType = flow.GetModel().GetType();

            //    var item = new FlowDataDetails
            //    { 
            //        RefId = flowType.FullName,
            //        FlowType = showNamespaces == true ? flowType.FullName : flowType.Name, 
            //        ModelType = showNamespaces == true ? modelType.FullName : modelType.Name,
            //        TaskCount = flow.Tasks.Count,
            //    };

            //    result.Data.Add(item);
            //}

            ////result.Data.Add(new FlowDataDetails { FlowName = "flow1" });
            //return result;
        }

    }

    public class RegisteredListForm : FormListBase<FlowDetailsModel>
    {
        protected override void Define(FormListBuilder<FlowDetailsModel> builder)
        {
            builder.List(p => p.Data, e =>
            {
                e.DisplayName = "Registered Flows";

                e.Property(p => p.RefId).IsPrimaryKey().IsHidden();
                e.Property(p => p.FlowType).Name("Flow");
                e.Property(p => p.FlowInterface).Name("Type");
                e.Property(p => p.ModelType).Name("Model");
                e.Property(p => p.TaskCount).Name("Tasks");

                e.ContextButton("View", "/_BlazorForms/admin/registered-flows/{0}");
            });
        }
    }
}
