using BlazorForms.Flows;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Domain.Repositories;

namespace CrmLightDemoApp.Onion.Infrastructure
{
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        public PersonRepository()
        {
            // pre fill some data
            _localCache.Add(new Person { Id = 1, FirstName = "Jack", LastName = "Wombat", BirthDate = new DateTime(1998, 10, 21) });
            _localCache.Add(new Person { Id = 2, FirstName = "David", LastName = "Jones", BirthDate = new DateTime(1978, 12, 1) });
            _localCache.Add(new Person { Id = 3, FirstName = "Louis", LastName = "Monero", BirthDate = new DateTime(2001, 3, 16) });
            _id = 10;
        }
    }
}
