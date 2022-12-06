using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Platform.Crm.Domain.Models.Artel;
using BlazorForms.Shared.Extensions;

namespace BlazorForms.Platform.Crm.Definitions.Services
{
    public interface IArtelProjectService
    {
        Task<List<ArtelProjectDetails>> GetProjects(QueryOptions queryOptions);
        Task<List<ArtelMemberDetails>> GetProjectMemberList(int projectId, QueryOptions queryOptions);
    }
}
