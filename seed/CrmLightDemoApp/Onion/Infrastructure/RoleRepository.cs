using BlazorForms.Flows;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain.Entities;
using CrmLightDemoApp.Onion.Domain.Repositories;

namespace CrmLightDemoApp.Onion.Infrastructure
{
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
	{
        public RoleRepository() 
        {
            // pre fill some data
            _localCache.Add(new Role { Id = 1, Name = "Admin" });
            _localCache.Add(new Role { Id = 2, Name = "Support" });
            _localCache.Add(new Role { Id = 3, Name = "Operations" });
            _localCache.Add(new Role { Id = 4, Name = "Manager" });
            _localCache.Add(new Role { Id = 5, Name = "Executive" });
            _id = 10;
        }
    }
}
