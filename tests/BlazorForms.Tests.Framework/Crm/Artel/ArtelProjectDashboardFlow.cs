using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Platform.Crm.Artel;
using BlazorForms.Platform.Crm.Domain.Models.Artel;

namespace BlazorForms.Platform.Crm.Business.Artel
{
    [Flow(nameof(ArtelProjectDashboardFlow), DefaultReadonlyView = typeof(FormArtelProjectDashboard))]
    public class ArtelProjectDashboardFlow : FluentFlowBase<ArtelProjectDashboardModel>
    {
        public override void Define()
        {
            this
                .Begin()
                .Next(LoadData)
                .NextForm(typeof(FormArtelProjectDashboard))
                .Next(SaveAsync)
                .End();
        }

        private async Task LoadData()
        {
            Model.Project = new ArtelProjectDetails() { Id = Params.ItemKey, Name = "Collective Project Ownership", BaseCurrencySearch = "USD", InitialSharePrice = new Money() { Amount = 100, Currency = "USD" },
                Statistics = new ArtelProjectStatistics() { TotalValue = new Money(){ Amount = 150000, Currency = "USD"}, TotalSharesIssued = 30000, 
                    CurrentBalance = new Money() { Amount = 1057, Currency = "USD" }, TotalTimesheetHours = 1960, MemberCount = 3
                }
            };
            Model.Members = (new ArtelMemberDetails[]
            {
                new ArtelMemberDetails()
                {
                    EntityId = 10, FirstName = "Ev", LastName = "Uklad", Email = "ev.uklad@gmail.com", AssignedRole = "Developer", IsAdmin = true,
                    Statistics = new ArtelMemberStatistics(){ OutstandingBalance = new Money(10, "USD"), TotalPaid = new Money(2300, "USD"), TotalShares = 300, TotalTimesheetHours = 320}
                },
                new ArtelMemberDetails()
                {
                    EntityId = 11, FirstName = "James", LastName = "Frank", Email = "j.frank@gmail.com", AssignedRole = "Investor", IsAdmin = false,
                    Statistics = new ArtelMemberStatistics(){ OutstandingBalance = new Money(0, "USD"), TotalPaid = new Money(0, "USD"), TotalShares = 3000, TotalTimesheetHours = 20}
                },
                new ArtelMemberDetails()
                {
                    EntityId = 12, FirstName = "Michaylo", LastName = "Shmalko", Email = "m.shmalko@gmail.com", AssignedRole = "Project manager", IsAdmin = false,
                    Statistics = new ArtelMemberStatistics(){ OutstandingBalance = new Money(1100, "USD"), TotalPaid = new Money(300, "USD"), TotalShares = 300, TotalTimesheetHours = 310}
                },
            }).ToList(); 
        }

        private async Task SaveAsync()
        {

        }
    }
}
