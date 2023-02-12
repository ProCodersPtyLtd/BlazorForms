using BlazorForms.Flows;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain.Entities;
using CrmLightDemoApp.Onion.Domain.Repositories;

namespace CrmLightDemoApp.Onion.Infrastructure
{
    public class UserRoleLinkRepository : RepositoryBase<UserRoleLink>, IUserRoleLinkRepository
	{
        public UserRoleLinkRepository() 
        {
            // pre fill some data
            _localCache.Add(new UserRoleLink { Id = 1, UserId = 1, RoleId = 1 });
            _id = 10;
        }

		public async Task<List<UserRoleLink>> GetAllByUserIdAsync(int userId)
		{
            var list = _localCache.Where(x => !x.Deleted && x.UserId == userId).ToList();
            return list;
		}
	}
}
