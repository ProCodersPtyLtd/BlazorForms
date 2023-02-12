using CrmLightDemoApp.Onion.Domain.Entities;

namespace CrmLightDemoApp.Onion.Domain.Repositories
{
    public interface IUserRoleLinkRepository : IRepository<UserRoleLink>
    {
		Task<List<UserRoleLink>> GetAllByUserIdAsync(int userId);
	}
}
