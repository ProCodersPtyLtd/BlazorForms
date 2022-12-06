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

namespace BlazorForms.Admin.BusinessObjects.StorageFlows
{
    public class StoredListFlow : FluentFlowBase<FlowDetailsModel>
    {
        private readonly IFlowDataProvider _flowDataProvider;

        public StoredListFlow(IFlowDataProvider flowDataProvider)
        {
            _flowDataProvider = flowDataProvider;
        }

        public override void Define()
        {
            this.ListForm(typeof(StorageListForm), ViewDataCallbackTask, true);
        }

        public async Task<FlowDetailsModel> ViewDataCallbackTask(QueryOptions queryOptions)
        {
            var result = new FlowDetailsModel();
            var options = new FlowDataOptions();
            options.ShowNamespaces = Params.GetParam("ShowNamespaces").AsBool();
            result.Data = await _flowDataProvider.GetStoredFlows(queryOptions, options);
            return result;
        }

    }

    public class StorageListForm : FormListBase<FlowDetailsModel>
    {
        protected override void Define(FormListBuilder<FlowDetailsModel> builder)
        {
            builder.List(p => p.Data, e =>
            {
                e.DisplayName = "Registered Flows";

                e.Property(p => p.RefId).IsPrimaryKey();
                e.Property(p => p.FlowType).Name("Flow");
                e.Property(p => p.ModelType).Name("Model");
                e.Property(p => p.TaskCount).Name("Tasks");
                e.Property(p => p.FlowState).Name("State");
                e.Property(p => p.ResultState).Name("Result");

                e.ContextButton("View", "/_BlazorForms/admin/registered-flows/{0}");
            });
        }
    }
}
