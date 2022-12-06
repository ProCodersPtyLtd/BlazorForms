using BlazorForms.Platform;
using BlazorForms.Platform.Shared.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorForms.Tests.Framework.Core
{
    public class TestAuthState : IAuthState
    {
        public string UserLoginOverride { get; set; } = "login@mail.com.au";
        public async Task<string> UserLogin() 
        {
            Console.WriteLine($"TestAuthState.UserLogin() returned {UserLoginOverride}");
            return UserLoginOverride;
        }

        public async Task<ClaimsPrincipal> User() => throw new NotImplementedException();
        public async Task SetLogin(string login) => throw new NotImplementedException();
    }
}
