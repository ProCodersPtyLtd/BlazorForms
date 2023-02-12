using BlazorForms.FlowRules;
using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms.Platform.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Platform.Tests.Flows;

namespace BlazorForms.Platform.Tests.Flows
{
    [Flow(nameof(DemoClientTableParamsFlow))]
    public class DemoClientTableParamsFlow : FlowBase<DemoClientTableModel>
    {
        public override async Task ExecuteFlow()
        {
            BeginTask();

            if (1 == 1)
            {
                Model.TableParamTop = 14;
            }

            await UserView(typeof(DemoClientTableParamsForm), ViewDataCallbackTask);

            EndTask();
        }

        public virtual void BeginTask()
        { }

        public virtual DemoClientTableModel ViewDataCallbackTask(QueryOptions queryOptions, dynamic dynamicParams)
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
                        PostCode = $"{2000 + idx}"
                    },
                    CompanyName = $"Sample Company #{idx}"
                })
            };

            if (dynamicParams.Top != null)
            {
                data.CompaniesCount = dynamicParams.Top;
                data.Companies = data.Companies.Take(data.CompaniesCount);
            }

            return data;
        }

        public virtual DemoClientTableModel ViewDataCallbackTask2(QueryOptions queryOptions, dynamic dynamicParams)
        {
            var data = new DemoClientTableModel
            {
                CompaniesCount = 234,
                Companies = Enumerable
                .Range(1 + queryOptions.PageIndex * queryOptions.PageSize, queryOptions.PageSize)
                .Select(idx => new CompanyTableRow
                {
                    Key = idx.ToString(),
                })
            };

            data.CompaniesCount = Model.TableParamTop;
            data.Companies = data.Companies.Take(data.CompaniesCount);

            return data;
        }

        public virtual void EndTask()
        { }

        public override Type GetModelType()
        {
            throw new NotImplementedException();
        }
    }

    public class DemoClientTableParamsForm : FlowTaskDefinitionBase<DemoClientTableModel>
    {
        public override string Name => "TestForm2";

        [FlowRule(typeof(DemoClientTableParamsRule), FormRuleTriggers.Loaded)]
        [FlowRule(typeof(DemoClientTableParamsRule2), FormRuleTriggers.Loaded)]
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

    public class DemoClientTableParamsRule : FlowRuleBase<DemoClientTableModel>
    {
        public override string RuleCode => "DEMO-1";

        public override void Execute(DemoClientTableModel model)
        {
            dynamic bag = model.Bag;
            bag.Top = 5;
        }
    }

    public class DemoClientTableParamsRule2 : FlowRuleBase<DemoClientTableModel>
    {
        public override string RuleCode => "DEMO-2";

        public override void Execute(DemoClientTableModel model)
        {
            model.TableParamTop = 27;
        }
    }
}
