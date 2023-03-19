using CrmLightDemoApp.Store.Onion.Services.Model;

namespace CrmLightDemoApp.Store.Onion.Services.Abstractions
{
    public interface IUserService
    {
        Task<List<UserModel>> GetAllUserDetailsAsync();
        Task<UserModel> GetUserDetailsAsync(int id);
    }
}
