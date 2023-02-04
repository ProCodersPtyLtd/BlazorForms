using BlazorForms.Flows;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Domain.Entities;
using CrmLightDemoApp.Onion.Domain.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CrmLightDemoApp.Onion.Infrastructure
{
    public class UserRepository : LocalCacheRepository<User>, IUserRepository
    {
		private readonly IPersonRepository _personRepository;

		public UserRepository(IPersonRepository personRepository)
        {
			_personRepository = personRepository;

			// pre fill some data
			_localCache.Add(new User { Id = 1, PersonId = 3, TenantAccountId = 1, Login = "Louis.Monero@bebemo.ch" });
			_localCache.Add(new User { Id = 2, PersonId = 4, TenantAccountId = 1 });
			_localCache.Add(new User { Id = 3, PersonId = 5, TenantAccountId = 1 });
			_localCache.Add(new User { Id = 4, PersonId = 6, TenantAccountId = 1 });
			_localCache.Add(new User { Id = 5, PersonId = 7, TenantAccountId = 1 });
			_localCache.Add(new User { Id = 6, PersonId = 8, TenantAccountId = 1 });
			_localCache.Add(new User { Id = 7, PersonId = 9, TenantAccountId = 1 });
			_localCache.Add(new User { Id = 8, PersonId = 10, TenantAccountId = 1 });
			_localCache.Add(new User { Id = 9, PersonId = 11, TenantAccountId = 1 });
			_localCache.Add(new User { Id = 10, PersonId = 12, TenantAccountId = 1 });
            _id = 15;
        }

		public async Task<List<UserDetails>> GetAllUserDetailsAsync()
		{
			var list = _localCache.Where(x => !x.Deleted).Select(x =>
			{
				var item = new UserDetails();
				x.ReflectionCopyTo(item);
				return item;
			}).ToList();

			var personIds = list.Select(x => x.PersonId).Distinct().ToList();
			var persons = (await _personRepository.GetListByIdsAsync(personIds)).ToDictionary(x => x.Id, x => x);

			foreach (var item in list)
			{
				item.PersonFullName = $"{persons[item.PersonId].FirstName} {persons[item.PersonId].LastName}";
			}

			return list;
		}
	}
}
