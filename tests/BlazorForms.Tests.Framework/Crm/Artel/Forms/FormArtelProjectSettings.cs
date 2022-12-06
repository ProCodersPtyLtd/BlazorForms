using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.FlowRules;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Platform.Crm.Artel;
using BlazorForms.Platform.ProcessFlow;

namespace BlazorForms.Platform.Crm.Business.Artel
{
    [Form("Project Settings")]
    public class FormArtelProjectSettings : FlowTaskDefinitionBase<ArtelProjectSettingsModel>
    {
        [FormComponent(typeof(TextEdit))]
        [Display("Name")]
        public object NameControl => ModelProp(m => m.Project.Name);

        [FlowRule(typeof(CurrencyEntered), FormRuleTriggers.Changed)]
        [FormComponent(typeof(Autocomplete))]
        [Display("Base Currency")]
        public object ProjectBaseCurrencyControl => EditWithOptions(a => a.Project.BaseCurrencySearch, e => e.CurrencyListRef, m => m.ShortName);

        [FormComponent(typeof(MoneyEdit))]
        [Display("Initial Share Price")]
        public object SharePriceControl => ModelProp(m => m.Project.InitialSharePrice);

        [FormComponent(typeof(PercentEdit))]
        [Display("Default Shares/Money Payment Proportion")]
        public object ProportionControl => ModelProp(m => m.Project.DefaultSharesPaymentProportionPercent);


        [FormComponent(typeof(DropDown))]
        [Display("Salary Payment Frequency")]
        public object FreqControl => SingleSelect(t => t.Project.PaymentFrequencyCode, p => p.FrequencyRef, m => m.Code, m => m.Name);

        [FormComponent(typeof(TextEdit))]
        [Display("Day of Month")]
        public object DayControl => ModelProp(m => m.Project.PaymentFrequencyDay);

        // Roles table
        [FlowRule(typeof(ProjectRoleDefaultRule), FormRuleTriggers.ItemAdded)]
        [FormComponent(typeof(Repeater))]
        [Display("Project Roles")]
        public IFieldBinding Members => Repeater(t => t.Roles);

        [FormComponent(typeof(TextEdit))]
        [Display("Role", Required = true)]
        public IFieldBinding RoleControl => TableColumn(t => t.Roles, m => m.Name);

        [FormComponent(typeof(MoneyEdit))]
        [Display("Hourly rate", Required = true)]
        public IFieldBinding RateControl => TableColumn(t => t.Roles, m => m.HourlyRate);

        [Display("Cancel")]
        public object CancelButton => ActionButton(ActionType.Close);
        [Display("Submit")]
        public object SubmitButton => ActionButton(ActionType.Submit);
    }

    public class CurrencyEntered : FlowRuleAsyncBase<ArtelProjectSettingsModel>
    {
        public override string RuleCode => "APS-CUR-1";

        public override async Task Execute(ArtelProjectSettingsModel model)
        {
            model.Project.InitialSharePrice.Currency = model.Project.BaseCurrencySearch;

            foreach (var role in model.Roles)
            {
                role.HourlyRate.Currency = model.Project.BaseCurrencySearch;
            }
        }
    }

    public class ProjectRoleDefaultRule : FlowRuleBase<ArtelProjectSettingsModel>
    {
        public override string RuleCode => "APS-DFT-1";

        public override void Execute(ArtelProjectSettingsModel model)
        {
            model.Roles[RunParams.RowIndex].HourlyRate = new Money{ Currency = model.Project.BaseCurrencySearch };
        }
    }
}
