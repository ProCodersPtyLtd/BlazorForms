using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using BlazorForms.Platform.ProcessFlow;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Tests.Flows
{
    [Flow(nameof(DemoClientTableFlow))]
    public class DemoClientTableFlow : FluentFlowBase<DemoClientTableModel>
    {
        public override void Define()
        {
            this.ListForm(typeof(DemoClientTableForm), ViewDataCallbackTask);
        }

        [Task]
        public virtual void BeginTask()
        { }

        [Task]
        public virtual async Task<DemoClientTableModel> ViewDataCallbackTask(QueryOptions queryOptions)
        {
            var data = new DemoClientTableModel
            {
                CompaniesCount = 234,
                Companies = Enumerable
                .Range(1 + queryOptions.PageIndex * queryOptions.PageSize, queryOptions.PageSize)
                .Select(idx => new CompanyTableRow
                {
                    Key = idx.ToString(),
                    ABN = $"35 164 943 {500 + idx}",
                    Address = new StreetAddress
                    {
                        PostCode = $"{2000+idx}"
                    },
                    CompanyName = $"Sample Company #{idx}"
                })
            };
            return data;
        }

        [Task]
        public virtual void EndTask()
        { }
    }

    public class DemoClientTableModel : FlowModelBase
    {
        /// <summary>
        /// This must be called Items as frontent will be looking for items property
        /// </summary>
        public virtual IEnumerable<CompanyTableRow> Companies { get; set; }
        /// <summary>
        /// Total number of rows in the view
        /// </summary>
        public virtual int CompaniesCount { get; set; }

        public virtual int TableParamTop { get; set; }
    }

    public class CompanyTableRow
    {
        virtual public string Key { get; set; }
        virtual public string CompanyName { get; set; }
        virtual public string ABN { get; set; }
        virtual public StreetAddress Address { get; set; }
        virtual public string SecretId { get; set; }
    }

    public class StreetAddress
    {
        virtual public string Line1 { get; set; }
        virtual public string Line2 { get; set; }
        virtual public string Line3 { get; set; }
        virtual public string City { get; set; }
        virtual public string PostCode { get; set; }
        virtual public string CountryCode { get; set; }
    }

    public class DemoClientTableParams : FlowParamsBase
    {
        public string UserId { get; set; }
    }

    public class DemoClientTableForm : FlowTaskDefinitionBase<DemoClientTableModel>
    {
        public override string Name => "TestForm2";

        [Display("Key", Visible = false, IsPrimaryKey = true)]
        public object Key => TableColumn(t => t.Companies, c => c.Key);

        [FormComponent(typeof(TextEdit))]
        [Display("Company Name", Required = true)]
        public object CompanyName => TableColumn(t => t.Companies, c => c.CompanyName);

        [FormComponent(typeof(TextEdit))]
        [Display("ABN", Required = true)]
        public object Abn => TableColumn(t => t.Companies, c => c.ABN);

        [FormComponent(typeof(TextEdit))]
        [Display("Street Address", Required = true)]
        public object StreetAddress => TableColumn(t => t.Companies, c => c.Address.Line1 + c.Address.City);

        public object CompaniesCount => TableCount(t => t.Companies, c => c.CompaniesCount);
    }
}
