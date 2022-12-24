using BlazorForms.Flows;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Domain.Repositories;

namespace CrmLightDemoApp.Onion.Infrastructure
{
    // this is repository emulator that stores all data in memory
    // it stores and retrieves object copies, like a real database
    public class PersonCompanyRepository : IPersonCompanyRepository
    {
        private int _id = 0;
        private readonly List<PersonCompanyLink> _personCompanyLinkCache = new List<PersonCompanyLink>();

        public async Task<int> CreateAsync(PersonCompanyLink data)
        {
            _id++;
            data.Id = _id;
            _personCompanyLinkCache.Add(data.GetCopy());
            return _id;
        }

        public async Task DeleteAsync(int id)
        {
            _personCompanyLinkCache.Remove(_personCompanyLinkCache.Single(x => x.Id == id));
        }

        public async Task<PersonCompanyLink> GetByIdAsync(int id)
        {
            return _personCompanyLinkCache.Single(x => x.Id == id).GetCopy();
        }

        public async Task<List<PersonCompanyLink>> GetAllAsync()
        {
            return _personCompanyLinkCache.Select(x => x.GetCopy()).ToList();
        }

        public async Task UpdateAsync(PersonCompanyLink data)
        {
            await DeleteAsync(data.Id);
            _personCompanyLinkCache.Add(data.GetCopy());
        }

        public async Task<List<PersonCompanyLinkDetails>> GetByPersonIdAsync(int personId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PersonCompanyLinkDetails>> GetByCompanyIdAsync(int companyId)
        {
            throw new NotImplementedException();
        }
    }
}
