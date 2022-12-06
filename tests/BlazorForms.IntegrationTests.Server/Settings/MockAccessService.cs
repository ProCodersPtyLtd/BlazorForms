using BlazorForms.Platform.Crm.Definitions.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorForms.Integration.Tests.Server
{
    internal class MockAccessService : IAccessService
    {
        public async Task<IEnumerable<string>> GetUserClaims(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckClaim(string email, string claim)
        {
            if ((email == "appr1.timesheets@mail.com.au" || email == "timesheets.admin@irisriver.com") && claim == "Approver")
            {
                return true;
            }
            return false;
        }

        public Task UpdateUserClaims(string email, IEnumerable<string> claims)
        {
            throw new NotImplementedException();
        }
    }
}