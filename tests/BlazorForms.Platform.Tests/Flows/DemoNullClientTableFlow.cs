using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms.Platform.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorForms.Platform.Tests.Flows;

namespace BlazorForms.Platform.Tests.Flows
{
    [Flow(nameof(DemoNullClientTableFlow))]
    public class DemoNullClientTableFlow : FluentFlowBase<DemoClientTableModel>
    {
        public override void Define()
        {
            this.ListForm(typeof(DemoNullClientTableForm), ViewDataCallbackTask);
        }

        [Task]
        public virtual void BeginTask()
        { }

        [ResolveTableData]
        public async virtual Task<DemoClientTableModel> ViewDataCallbackTask(QueryOptions queryOptions)
        {
            int size = 15;
            int start = queryOptions.PageIndex * queryOptions.PageSize;
            int end = (queryOptions.PageIndex + 1) * queryOptions.PageSize;
            var data = new DemoClientTableModel();
            var list = new List<CompanyTableRow>();

            for (int i = start; i < Math.Min(size, end); i++)
            {
                list.Add(new CompanyTableRow
                {
                    ABN = $"1234567890{i}",
                    CompanyName = $"company{i}"
                });
            };

            data.Companies = list;
            return data;
        }

        [Task]
        public virtual void EndTask()
        { }

    }

    public class DemoNullClientTableForm : FlowTaskDefinitionBase<DemoClientTableModel>
    {
        // Form bindings
        public override string Name => "TestForm2";

        [Display("Company Name", Required = true)]
        public object CompanyName => TableColumn(t => t.Companies, c => c.CompanyName);

        [Display("ABN", Required = true)]
        public object Abn => TableColumn(t => t.Companies, c => c.ABN);

        [Display("Street Address", Required = true)]
        public object StreetAddress => TableColumn(t => t.Companies, c => c.Address.Line1);
    }
}
