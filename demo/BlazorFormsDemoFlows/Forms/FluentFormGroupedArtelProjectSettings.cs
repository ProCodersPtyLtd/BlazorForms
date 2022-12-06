using BlazorForms.Forms;
using BlazorForms.Platform.Crm.Artel;
using BlazorForms.Platform.Crm.Business.Artel;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorFormsDemoFlows.Forms
{
    public class FluentFormGroupedArtelProjectSettings : FormEditBase<ArtelProjectSettingsModel>
    {
        protected override void Define(FormEntityTypeBuilder<ArtelProjectSettingsModel> f)
        {
            f.Group("Left");

            f.DisplayName = "Project Settings - Fluent Form";

            f.Property(p => p.Project.Name).IsRequired();

            f.Property(p => p.Project.StartDate);

            f.Property(p => p.Project.BaseCurrencySearch).EditWithOptions(e => e.CurrencyListRef, m => m.ShortName)
                .Label("Base Currency").Control(typeof(Autocomplete)).Rule(typeof(CurrencyEntered));

            f.Property(p => p.Project.InitialSharePrice).Label("Initial Share Price").Control(typeof(MoneyEdit));

            f.Group("Right");

            f.Property(p => p.Project.DefaultSharesPaymentProportionPercent).Label("Default Shares/Money Payment Proportion").Control(typeof(PercentEdit));

            f.Property(p => p.Project.PaymentFrequencyCode).Dropdown(p => p.FrequencyRef, m => m.Code, m => m.Name);
            f.Property(p => p.Project.PaymentFrequencyDay).Label("Day of Month").Control(typeof(TextEdit));
            f.Property(p => p.Project.PaymentNotification).Label("Notify on Payment Sent");

            f.Property(p => p.Project.RoadmapAttached).Label("Roadmap Attached").Control(typeof(FileUpload));

            f.Group("Bottom");

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
}
