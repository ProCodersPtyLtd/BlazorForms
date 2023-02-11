using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services.Abstractions
{
    public interface IUserService
    {
        Task<List<UserModel>> GetAllUserDetailsAsync();
        Task<UserModel> GetUserDetailsAsync(int id);
    }
}
