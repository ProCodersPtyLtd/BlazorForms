using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Platform.Crm.Artel;
using BlazorForms.Platform.Crm.Business.Artel;
using BlazorForms.Platform.Crm.Domain.Models.Artel;
using BlazorForms.Platform.ProcessFlow;

namespace BlazorFormsDemoFlows
{
    public class FluentFormArtelProjectSettings : FormEditBase<ArtelProjectSettingsModel>
    {
        protected override void Define(FormEntityTypeBuilder<ArtelProjectSettingsModel> f)
        {
            f.DisplayName = "Project Settings - Fluent Form";

            f.Property(p => p.Project.Name).IsRequired();

            f.Property(p => p.Project.StartDate).IsRequired();

            f.Property(p => p.Project.BaseCurrencySearch).EditWithOptions(e => e.CurrencyListRef, m => m.ShortName)
                .IsRequired().Label("Base Currency").Control(typeof(Autocomplete)).Rule(typeof(CurrencyEntered));

            f.Property(p => p.Project.InitialSharePrice).IsRequired().Label("Initial Share Price").Control(typeof(MoneyEdit));
            f.Property(p => p.Project.DefaultSharesPaymentProportionPercent).IsRequired().Label("Default Shares/Money Payment Proportion").Control(typeof(PercentEdit));

            f.Property(p => p.Project.PaymentFrequencyCode).IsRequired().Dropdown(p => p.FrequencyRef, m => m.Code, m => m.Name);
            f.Property(p => p.Project.PaymentFrequencyDay).Label("Day of Month").Control(typeof(TextEdit));
            f.Property(p => p.Project.PaymentNotification).Label("Notify on Payment Sent");

            f.Property(p => p.Project.RoadmapFile).Label("Roadmap Attached").Control(typeof(FileUpload));

            f.Repeater(t => t.Roles, e =>
            {
                e.DisplayName = "Project Roles";

                e.Property(p => p.Name).IsRequired().Label("Role")
                    .Rule(typeof(ProjectRoleNameEmptyRule), FormRuleTriggers.ItemChanged);

                e.Property(p => p.HourlyRate).IsRequired().Label("Hourly Rate").Control(typeof(MoneyEdit));

                e.PropertyRoot(p => p.FrequencyCode).Dropdown(p => p.FrequencyRef, m => m.Code, m => m.Name).Label("Frequency")
                    .Rule(typeof(FrequencyDefaultRule), FormRuleTriggers.ItemChanged);

            }).Rule(typeof(ProjectRoleDefaultRule), FormRuleTriggers.ItemAdded);

            f.Button("/", ButtonActionTypes.Close, "Cancel");
            f.Button("/", ButtonActionTypes.Submit);
            f.Button("/", ButtonActionTypes.Custom, "Blinking");

            f.Rule(typeof(SampleProjectSettingsFormLevelExampleRule));
        }
    }

    public class FrequencyDefaultRule : FlowRuleBase<ArtelProjectSettingsModel>
    {
        public override string RuleCode => "APS-FREQ-1";

        public override void Execute(ArtelProjectSettingsModel model)
        {
            if (model.Roles[RunParams.RowIndex].FrequencyCode == "Mon")
            {
                model.Roles[RunParams.RowIndex].Name = "Incognito";
                model.Roles[RunParams.RowIndex].HourlyRate.Amount = 9.99m;
            }
        }
    }

    public class SampleProjectSettingsFormLevelExampleRule : FlowRuleAsyncBase<ArtelProjectSettingsModel>
    {
        private static bool _blink = true;

        public override string RuleCode => "SLR-005";

        public override async Task Execute(ArtelProjectSettingsModel model)
        {
            _blink = !_blink;
            Result.Fields[ModelBinding.CustomButtonBinding].Visible = _blink;
        }
    }

    public class FluentFormArtelProjectSettingsSaved : FormEditBase<ArtelProjectSettingsModel>
    {
        protected override void Define(FormEntityTypeBuilder<ArtelProjectSettingsModel> f)
        {
            f.DisplayName = "Project Settings - Fluent Form";

            f.Property(p => p.Message).IsReadOnly().Control(typeof(Header));

            f.Button("/", ButtonActionTypes.Submit, "OK");
        }
    }

    public class FluentArtelProjectSettingsFlow : FluentFlowBase<ArtelProjectSettingsModel>
    {
        public override void Define()
        {
            this
                .Begin()
                .Next(LoadData)
                .NextForm(typeof(FluentFormArtelProjectSettings))
                .Next(SaveAsync)
                .NextForm(typeof(FluentFormArtelProjectSettingsSaved))
                .End();
        }

        private async Task LoadData()
        {
            Model.Project = new ArtelProjectDetails
            {
                //Name = "Project1",
                StartDate= DateTime.Now.Date.ToUniversalTime(),
                BaseCurrencySearch = "USD",
                PaymentFrequencyDay = 1,
                DefaultSharesPaymentProportionPercent = 50m,
                InitialSharePrice = new Money() { Amount = 100, Currency = "USD" }
            };

            Model.CurrencyListRef = (new Currency[] { new Currency { ShortName = "AUD" }, new Currency { ShortName = "USD" }, new Currency { ShortName = "BTC" }, }).ToList();

            Model.FrequencyRef = (new FrequencyTypeDetails[] { new FrequencyTypeDetails{ Name = "Monthly", Code = "Mon"}
                , new FrequencyTypeDetails { Name = "Fortnightly", Code = "Frn" }, new FrequencyTypeDetails{ Name = "Weekly", Code = "Wek"} }).ToList();

            Model.Roles = (new ArtelRoleDetails[]
            {
                new ArtelRoleDetails { Name = "Investor", HourlyRate = new Money{ Amount = 0, Currency = "USD"} },
                new ArtelRoleDetails { Name = "Project manager", HourlyRate = new Money{ Amount = 100, Currency = "USD"} },
                new ArtelRoleDetails { Name = "Business analyst", HourlyRate = new Money{ Amount = 100, Currency = "USD"} },
                new ArtelRoleDetails { Name = "Developer", HourlyRate = new Money{ Amount = 100, Currency = "USD"} },
                new ArtelRoleDetails { Name = "Tester", HourlyRate = new Money{ Amount = 100, Currency = "USD"} },
                new ArtelRoleDetails { Name = "Junior", HourlyRate = new Money{ Amount = 0, Currency = "USD"} },
            }).ToList();
        }

        private async Task SaveAsync()
        {
            var isUtc = Model.Project.StartDate.Value.Kind == DateTimeKind.Utc;
            Model.Message = "Form submitted successfully.";
        }
    }
}
