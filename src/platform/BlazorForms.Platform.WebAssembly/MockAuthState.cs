using BlazorForms.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorForms.Stubs
{
    public class MockAuthState : IAuthState
    {
        public string UserLoginOverride { get; set; } = "user1@mail.com.au";
        public async Task<string> UserLogin() => UserLoginOverride;

        public async Task<ClaimsPrincipal> User() => throw new NotImplementedException();
        public async Task SetLogin(string login) => throw new NotImplementedException();
    }
}
