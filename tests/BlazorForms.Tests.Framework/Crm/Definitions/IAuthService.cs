using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Platform.Crm.Domain.Models;
using BlazorForms.Platform.Crm.Domain.Models.Timesheets;

namespace BlazorForms.Platform.Crm.Definitions.Services
{
    public interface IAuthService
    {
        Task<AuthUser> GetCurrentUser();
        Task<string> GetCurrentUserName();
        Task<UserDetails> GetUserByEmail(string employeeEmail);
    }
}
