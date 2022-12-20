using BlazorForms.FlowRules;
using BlazorForms.Flows.Engine.Fluent;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared.Extensions;
using BlazorForms.Shared;
using BlazorForms.Flows.Definitions;
using System.Linq;

namespace MudBlazorUIDemo.Flows
{
    // short notation for list flows
    public class SampleListLargeFlow : ListFlowBase<CustomerListModel, TestCustAddrCountFormList>
    {
        static List<CustomerModel> _bigData = new List<CustomerModel>();

        static SampleListLargeFlow()
        {
            var gen = new RandomNameGenerator();

            for (int i = 0; i < 10000; i++)
            {
                gen.Run();

                _bigData.Add(new CustomerModel 
                { 
                    CustomerId = i + 1001,
                    FirstName = gen.FirstName,
                    LastName = gen.LastName,
                    DOB = gen.DOB
                });
            }
        }

        public override async Task<CustomerListModel> LoadDataAsync(QueryOptions queryOptions)
        {
            var result = new CustomerListModel();

            // query server data and take only 1K records
            var data = _bigData.ToList();

            if (!string.IsNullOrWhiteSpace(queryOptions.SearchString))
            {
                data = data.Where(r =>  r.FirstName.Contains(queryOptions.SearchString, StringComparison.OrdinalIgnoreCase)
                                        || r.LastName.Contains(queryOptions.SearchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (queryOptions.AllowPagination)
            {
                result.Data = data.Skip(queryOptions.PageIndex * queryOptions.PageSize).Take(queryOptions.PageSize).ToList();
                queryOptions.PageReturnTotalCount = data.Count;
            }
            else
            {
                // return only first 200 rows if pagination is not used
                result.Data = data.Take(200).ToList();
                queryOptions.PageReturnTotalCount = result.Data.Count;
            }

            return result;
        }
    }

    public class TestCustAddrCountFormList : FormListBase<CustomerListModel>
    {
        protected override void Define(FormListBuilder<CustomerListModel> builder)
        {
            builder.List(p => p.Data, e =>
            {
                e.DisplayName = "Large List Form";

                e.Property(p => p.CustomerId).IsPrimaryKey();
                e.Property(p => p.FirstName).Label("First Name").Filter(FieldFilterType.TextStarts);
                e.Property(p => p.LastName).Label("Last Name").Filter(FieldFilterType.TextStarts);
                e.Property(p => p.DOB).Format("dd/MM/yyyy");
                e.Property(p => p.Phone);
                e.Property(p => p.EmailAddress);
                e.Property(p => p.CompanyName);

                e.ContextButton("Edit", typeof(SampleListLargeEditFlow), BlazorForms.Shared.FlowReferenceOperation.Edit);
                e.ContextButton("Navigation", "ListItemView/{0}");
                e.ContextButton("Invisible", "ListItemView/{0}");
                e.ContextButton("Invisible on click", "ListItemView/{0}");

                e.NavigationButton("Add", typeof(SampleListLargeEditFlow), BlazorForms.Shared.FlowReferenceOperation.DialogForm);
                e.NavigationButton("Next", "LastList");

                e.Rule(typeof(SampleListDisableButtonsRule), FormRuleTriggers.Loaded);
                e.Rule(typeof(SampleListDisableMenuActionsRule), FormRuleTriggers.Loaded);
                e.Rule(typeof(SampleListDisableMenuActionsOnClickRule), FormRuleTriggers.ContextMenuClicking);
            });
        }
    }

    public class CustomerListModel : FlowModelBase
    {
        public virtual List<CustomerModel> Data { get; set; }
    }

    public class CustomerModel : FlowModelBase
    {
        public Int32 CustomerId { get; set; }
        public String CompanyName { get; set; }
        public String EmailAddress { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public DateTime? DOB { get; set; }
        public String MiddleName { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool NameStyle { get; set; }
        public String PasswordHash { get; set; }
        public String PasswordSalt { get; set; }
        public String Phone { get; set; }
        public Guid Rowguid { get; set; }
        public String SalesPerson { get; set; }
        public String Suffix { get; set; }
        public String Title { get; set; }
        public virtual Int32 AddrCount { get; set; }
    }

    public class SampleListDisableButtonsRule : FlowRuleAsyncBase<CustomerListModel>
    {
        public override string RuleCode => "SLLR-001";

        public override async Task Execute(CustomerListModel model)
        {
            if (model.Data.Count > 0)
            {
                var path = $"{ModelBinding.FlowReferenceButtonsBinding}.Next";
                Result.Fields[path].Visible = false;
            }
        }
    }

    public class SampleListDisableMenuActionsRule : FlowRuleAsyncBase<CustomerListModel>
    {
        public override string RuleCode => "SLLR-002";

        public override async Task Execute(CustomerListModel model)
        {
            if (model.Data.Count > 0)
            {
                var path = $"{ModelBinding.ListFormContextMenuBinding}.Invisible";
                Result.Fields[path].Visible = false;
            }
        }
    }

    public class SampleListDisableMenuActionsOnClickRule : FlowRuleAsyncBase<CustomerListModel>
    {
        private static bool _flash;

        public override string RuleCode => "SLLR-003";

        public override async Task Execute(CustomerListModel model)
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
