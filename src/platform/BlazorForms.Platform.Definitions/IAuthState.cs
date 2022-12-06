using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform
{
    public interface IAuthState
    {
        Task<ClaimsPrincipal> User();
        Task<string> UserLogin();
        Task SetLogin(string login);
    }
}
