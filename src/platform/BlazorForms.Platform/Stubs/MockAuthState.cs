using BlazorForms.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Stubs
{
    public class MockAuthState : IAuthState
    {
        public string UserLoginOverride { get; set; } = "login@mail.com.au";
        public async Task<string> UserLogin() => UserLoginOverride;

        public async Task<ClaimsPrincipal> User() => throw new NotImplementedException();
        public async Task SetLogin(string login) => throw new NotImplementedException();
    }
}
