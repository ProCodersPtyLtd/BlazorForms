using BlazorForms.Flows;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Domain.Repositories;

namespace CrmLightDemoApp.Onion.Infrastructure
{
    // this is repository emulator that stores all data in memory
    // it stores and retrieves object copies, like a real database
    public class CompanyRepository : ICompanyRepository
    {
        private int _id = 0;
        private readonly List<Company> _companyCache = new List<Company>();

        public async Task<int> CreateAsync(Company data)
        {
            _id++;
            data.Id = _id;
            _companyCache.Add(data.GetCopy());
            return _id;
        }

        public async Task DeleteAsync(int id)
        {
            _companyCache.Remove(_companyCache.Single(x => x.Id == id));
        }

        public async Task<Company> GetByIdAsync(int id)
        {
            return _companyCache.Single(x => x.Id == id).GetCopy();
        }

        public async Task<List<Company>> GetAllAsync()
        {
            return _companyCache.Select(x => x.GetCopy()).ToList();
        }

        public async Task UpdateAsync(Company data)
        {
            await DeleteAsync(data.Id);
            _companyCache.Add(data.GetCopy());
        }
    }
}
