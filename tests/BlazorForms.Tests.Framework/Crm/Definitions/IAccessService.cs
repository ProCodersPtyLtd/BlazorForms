using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Crm.Definitions.Services
{
    public interface IAccessService
    {
        Task<IEnumerable<string>> GetUserClaims(string email);
        Task<bool> CheckClaim(string email, string claim);
        Task UpdateUserClaims(string email, IEnumerable<string> claims);
    }
}
