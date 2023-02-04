using CrmLightDemoApp.Onion.Domain.Entities;

namespace CrmLightDemoApp.Onion.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<List<UserDetails>> GetAllUserDetailsAsync();
    }
}
