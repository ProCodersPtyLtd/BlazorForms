using BlazorForms.Flows;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;

namespace CrmLightDemoApp.Onion.Infrastructure
{
    // this is repository emulator that stores all data in memory
    // it stores and retrieves object copies, like a real database
    public class PersonRepository : IPersonRepository
    {
        private int _id = 0;
        private readonly List<Person> _peopleCache = new List<Person>();

        public async Task<int> CreateAsync(Person data)
        {
            _id++;
            data.Id = _id;
            _peopleCache.Add(data.GetCopy());
            return _id;
        }

        public async Task DeleteAsync(int id)
        {
            _peopleCache.Remove(_peopleCache.Single(x => x.Id == id));
        }

        public async Task<Person> GetByIdAsync(int id)
        {
            return _peopleCache.Single(x => x.Id == id).GetCopy();
        }

        public async Task<List<Person>> GetAllAsync()
        {
            return _peopleCache.Select(x => x.GetCopy()).ToList();
        }

        public async Task UpdateAsync(Person data)
        {
            await DeleteAsync(data.Id);
            _peopleCache.Add(data.GetCopy());
        }

        public async Task<List<PersonDetails>> GetAllWithContactsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
