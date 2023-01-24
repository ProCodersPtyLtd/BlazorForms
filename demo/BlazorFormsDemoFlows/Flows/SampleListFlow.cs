using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Engine.Fluent;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using BlazorFormsDemoModels.Models;

namespace BlazorFormsDemoFlows.Flows
{
    // short notation for list flows
    public class SampleListShortFlow : ListFlowBase<CustAddrCountModel, TestCustAddrCountFormList>
    {
        public override async Task<CustAddrCountModel> LoadDataAsync(QueryOptions queryOptions)
        {
            var result = new CustAddrCountModel { Data = new List<CustAddrCount>() };
            result.Data.Add(new CustAddrCount { CustomerId = 101, FirstName = "Ben", LastName = "Jones", AddrCount = 4 });
            result.Data.Add(new CustAddrCount { CustomerId = 102, FirstName = "Bengal", LastName = "Tiger", AddrCount = 4 });
            return result;
        }
    }

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

                e.Rule(typeof(SampleListDisableButtonsRule), FormRuleTriggers.Loaded);
                e.Rule(typeof(SampleListDisableMenuActionsRule), FormRuleTriggers.Loaded);
                e.Rule(typeof(SampleListDisableMenuActionsOnClickRule), FormRuleTriggers.ContextMenuClicking);
            });
        }
    }

    public class SampleListDisableButtonsRule : FlowRuleAsyncBase<CustAddrCountModel>
    {
        public override string RuleCode => "SLR-001";

        public override async Task Execute(CustAddrCountModel model)
        {
            if (model.Data.Count > 0)
            {
                var path = $"{ModelBinding.FlowReferenceButtonsBinding}.Next";
                Result.Fields[path].Visible = false;
            }
        }
    }

    public class SampleListDisableMenuActionsRule : FlowRuleAsyncBase<CustAddrCountModel>
    {
        public override string RuleCode => "SLR-002";

        public override async Task Execute(CustAddrCountModel model)
        {
            if (model.Data.Count > 0)
            {
                var path = $"{ModelBinding.ListFormContextMenuBinding}.Invisible";
                Result.Fields[path].Visible = false;
            }
        }
    }

    public class SampleListDisableMenuActionsOnClickRule : FlowRuleAsyncBase<CustAddrCountModel>
    {
        private static bool _flash;

        public override string RuleCode => "SLR-003";

        public override async Task Execute(CustAddrCountModel model)
        {
            if (model.Data.Count > 0)
            {
                var path = $"{ModelBinding.ListFormContextMenuBinding}.Invisible on click";
                Result.Fields[path].Visible = _flash;
                _flash = !_flash;
            }
        }
    }
}
