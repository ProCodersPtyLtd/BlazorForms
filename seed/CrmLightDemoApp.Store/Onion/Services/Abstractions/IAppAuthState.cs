using CrmLightDemoApp.Store.Onion.Services.Model;

namespace CrmLightDemoApp.Store.Onion.Services.Abstractions
{
    public interface IAppAuthState
    {
        UserModel GetCurrentUser();
		TenantAccountModel GetCurrentTenantAccount();
    }
}
