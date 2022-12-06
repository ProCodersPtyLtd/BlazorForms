using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Shared.Extensions;
using BlazorForms.Platform.Crm.Definitions.Services;
using BlazorForms.Platform.Crm.Domain.Models.Artel;

namespace BlazorForms.Platform.Crm.Domain.Services
{
    public class ArtelProjectService : IArtelProjectService
    {
        public async Task<List<ArtelProjectDetails>> GetProjects(QueryOptions queryOptions)
        {
            // stub
            return new ArtelProjectDetails[]
            {
                new ArtelProjectDetails{ Id = 155, Name = "Collective Project Ownership", StartDate = new DateTime(2020,1,1),
                    Statistics = new ArtelProjectStatistics{ MemberCount = 3, TotalValue = new Money { Amount = 110.25m, Currency = "BTC"}}}
            }.ToList();
        }

        public async Task<List<ArtelMemberDetails>> GetProjectMemberList(int projectId, QueryOptions queryOptions)
        {
            // stub
            return new ArtelMemberDetails[]
            {
                new ArtelMemberDetails{ EntityId = 201, FirstName = "Kolya", LastName = "Ivanov",
                    Statistics = new ArtelMemberStatistics{ TotalTimesheetHours = 609, TotalShares = 3000, TotalPaid = new Money { Amount = 110.25m, Currency = "BTC"},
                        OutstandingBalance = new Money { Amount = 0.25m, Currency = "BTC"}}}
            }.ToList();
        }
    }
}
