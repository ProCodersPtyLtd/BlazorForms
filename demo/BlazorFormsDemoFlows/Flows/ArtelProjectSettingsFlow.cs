using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Platform.Crm.Artel;
using BlazorForms.Platform.Crm.Business.Artel;
using BlazorForms.Platform.Crm.Domain.Models.Artel;

namespace BlazorFormsDemoFlows
{
    [Flow(nameof(ArtelProjectSettingsFlow))]
    public class ArtelProjectSettingsFlow : FluentFlowBase<ArtelProjectSettingsModel>
    {
        public const string Delete = "DELETE";

        public override void Define()
        {
            this
                .Begin()
                .Next(LoadData)
                .NextForm(typeof(FormArtelProjectSettings))
                .Next(SaveAsync)
                .End();
        }

        private async Task LoadData()
        {
            Model.Project = new ArtelProjectDetails
            {
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

        }
    }

   
}
