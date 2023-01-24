using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using BlazorFormsDemoModels.Models;

namespace BlazorFormsDemoFlows.Flows
{
    public class SampleListFlowWithException : FluentFlowBase<CustAddrCountModel>
    {
        private static bool _blink = true;

        public override void Define()
        {
            this.ListForm(typeof(TestCustAddrCountFormList2), ViewDataCallbackTask, true);
        }

        public async Task<CustAddrCountModel> ViewDataCallbackTask(QueryOptions queryOptions)
        {
            _blink = !_blink;

            if (_blink)
            {
                throw new Exception("CardList Form Flow ERROR 4");
            }

            var result = new CustAddrCountModel { Data = new List<CustAddrCount>() };
            result.Data.Add(new CustAddrCount { CustomerId = 101, FirstName = "Ben", LastName = "Jones", AddrCount = 4 });
            return result;
        }
    }

    public class TestCustAddrCountFormList2 : FormListBase<CustAddrCountModel>
    {
        protected override void Define(FormListBuilder<CustAddrCountModel> builder)
        {
            builder.List(p => p.Data, e => 
            {
                e.DisplayName = "CardList Form V2";

                e.Property(p => p.CustomerId).IsPrimaryKey();
                e.Property(p => p.FirstName).Label("First Name").Filter(FieldFilterType.TextStarts);
                e.Property(p => p.LastName).Label("Last Name").Filter(FieldFilterType.TextStarts);
                e.Property(p => p.AddrCount);
                e.Property(p => p.Phone);
                e.Property(p => p.EmailAddress);
                e.Property(p => p.CompanyName);

                e.ContextButton("Edit", typeof(SampleListEditFlow), BlazorForms.Shared.FlowReferenceOperation.Edit);
                e.ContextButton("Navigation", "ListItemView/{0}");
                e.ContextButton("Invisible", "ListItemView/{0}");
                e.ContextButton("Invisible on click", "ListItemView/{0}");

                e.NavigationButton("Add", typeof(SampleListEditFlow), BlazorForms.Shared.FlowReferenceOperation.DialogForm);
                e.NavigationButton("Next", "LastList");

                e.Rule(typeof(SampleListExceptionRule), FormRuleTriggers.Loaded);
            });
        }
    }

    public class SampleListExceptionRule : FlowRuleAsyncBase<CustAddrCountModel>
    {
        public override string RuleCode => "SLR-008";

        public override async Task Execute(CustAddrCountModel model)
        {
            throw new Exception("CardList Form Loaded rule ERROR 3");
        }
    }
}
