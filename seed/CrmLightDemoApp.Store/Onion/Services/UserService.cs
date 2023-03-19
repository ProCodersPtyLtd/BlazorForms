using BlazorForms.Shared;
using CrmLightDemoApp.Store.Onion.Domain.Repositories;
using CrmLightDemoApp.Store.Onion.Services.Abstractions;
using CrmLightDemoApp.Store.Onion.Services.Model;

namespace CrmLightDemoApp.Store.Onion.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleLinkRepository _userRoleLinkRepository;

        public UserService(IUserRepository userRepository, IPersonRepository personRepository, IRoleRepository roleRepository,
            IUserRoleLinkRepository userRoleLinkRepository)
        {
            _userRepository = userRepository;
            _personRepository = personRepository;
            _roleRepository = roleRepository;
            _userRoleLinkRepository = userRoleLinkRepository;
        }

        public async Task<List<UserModel>> GetAllUserDetailsAsync()
        {
            var result = new List<UserModel>();
            var list = await _userRepository.GetAllUserDetailsAsync();
            var userIdList = list.Select(x => x.Id).ToList();
            var q = _userRoleLinkRepository.GetContextQuery();
            q.Query = q.Query.Where(x => userIdList.Contains(x.UserId));
            var roleLinks = await _userRoleLinkRepository.RunContextQueryAsync(q);
            var roles = await _roleRepository.GetAllAsync();

            foreach (var item in list)
            {
                var user = new UserModel();
                item.ReflectionCopyTo(user);
                var links = roleLinks.Where(x => x.UserId == user.Id);
                user.CombinedUserRoles = UserRoleLinkModel.CombineFullList(user.Id, links, roles);
                result.Add(user);   
            }

            return result;
        }

		public async Task<UserModel> GetUserDetailsAsync(int id)
		{
            var user = new UserModel();
			var roles = await _roleRepository.GetAllAsync();
			var item = await _userRepository.GetByIdAsync(id);
			// item and user have different types - we use reflection to copy similar properties
			item.ReflectionCopyTo(user);
			var links = await _userRoleLinkRepository.GetAllByUserIdAsync(id);
			user.CombinedUserRoles = UserRoleLinkModel.CombineFullList(id, links, roles);
            return user;
        }
	}
}
