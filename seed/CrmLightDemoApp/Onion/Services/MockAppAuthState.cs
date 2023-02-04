using CrmLightDemoApp.Onion.Services.Abstractions;
using CrmLightDemoApp.Onion.Services.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Claims;

namespace CrmLightDemoApp.Onion.Services
{
    // This is a simple mock class
    // Remember that any code can modify its private data using reflection
    public class MockAppAuthState : IAppAuthState
    {
        private UserModel _currentUser;

        public MockAppAuthState() 
        {
        }

        public UserModel GetCurrentUser()
        {
            return _currentUser;
        }

        // internal method to switch users in demo
        internal void SetCurrentUser(UserModel user)
        {
            _currentUser = user;
		}
    }
}
