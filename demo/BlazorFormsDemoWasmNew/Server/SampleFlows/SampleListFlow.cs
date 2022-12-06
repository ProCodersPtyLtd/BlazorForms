using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared.Extensions;
using BlazorForms.Platform.Shared.Attributes;
using BlazorForms;

namespace BlazorFormsDemoWasmNew.Server.SampleFlows
{
    public class SampleListFlow : FluentFlowBase<CustAddrCountModel>
    {
        public override void Define()
        {
            this.ListForm(typeof(TestCustAddrCountFormList), ViewDataCallbackTask, true);
        }

        public async Task<CustAddrCountModel> ViewDataCallbackTask(QueryOptions queryOptions)
        {
            var result = new CustAddrCountModel { Data = new List<CustAddrCount>() };
            result.Data.Add(new CustAddrCount { CustomerId = 101, FirstName = "Ben", LastName = "Jones", AddrCount = 4 });
            return result;
        }
    }

    public class TestCustAddrCountFormList : FormListBase<CustAddrCountModel>
    {
        protected override void Define(FormListBuilder<CustAddrCountModel> builder)
        {

            builder.List(p => p.Data, e => 
            { 
                e.Property(p => p.CustomerId).IsPrimaryKey();
                e.Property(p => p.FirstName).Label("Firts Name").Filter(FieldFilterType.TextStarts);
                e.Property(p => p.LastName).Label("Last Name").Filter(FieldFilterType.TextStarts);
                e.Property(p => p.AddrCount);
                e.Property(p => p.Phone);
                e.Property(p => p.EmailAddress);
                e.Property(p => p.CompanyName);

                //e.Rule();
                e.ContextButton("Edit", typeof(SampleListEditFlow), BlazorForms.Shared.FlowReferenceOperation.Edit);
                e.ContextButton("Navigation", "ListItemView/{0}");

                e.NavigationButton("Add", typeof(SampleListEditFlow), BlazorForms.Shared.FlowReferenceOperation.DialogForm);
                e.NavigationButton("Next", "LastList");


            });
        }
    }
}
