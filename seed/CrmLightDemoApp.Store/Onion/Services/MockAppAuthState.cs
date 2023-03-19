using CrmLightDemoApp.Store.Onion.Services.Abstractions;
using CrmLightDemoApp.Store.Onion.Services.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Claims;

namespace CrmLightDemoApp.Store.Onion.Services
{
    // This is a simple mock class
    // Remember that any code can modify its private data using reflection
    public class MockAppAuthState : IAppAuthState
    {
        private UserModel _currentUser;
        private TenantAccountModel _currentTenant;

        public MockAppAuthState() 
        {
        }

        public UserModel GetCurrentUser()
        {
            return _currentUser;
        }

		public TenantAccountModel GetCurrentTenantAccount()
		{
            return _currentTenant;
		}

        // internal method to switch users in demo
        internal void SetCurrentUser(UserModel user)
        {
            _currentUser = user;
		}

        // internal method to set Tenant in demo
        internal void SetCurrentTenantAccount(TenantAccountModel tenant)
        {
            _currentTenant = tenant;
		}
	}
}
