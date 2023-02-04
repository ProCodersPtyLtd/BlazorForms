using BlazorForms.Flows;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Domain.Entities;
using CrmLightDemoApp.Onion.Domain.Repositories;

namespace CrmLightDemoApp.Onion.Infrastructure
{
    public class TenantAccountRepository : LocalCacheRepository<TenantAccount>, ITenantAccountRepository
    {
		private readonly ICompanyRepository _companyRepository;

		public TenantAccountRepository(ICompanyRepository companyRepository)
        {
			_companyRepository = companyRepository;

            // pre fill some data
            _localCache.Add(new TenantAccount { Id = 1, CompanyId = 7 });
            _id = 10;
        }

		public async Task<TenantAccountDetails> GetTenantAccountDetailsAsync()
		{
			var record = (await GetAllAsync()).First();
			var result = new TenantAccountDetails();
			record.ReflectionCopyTo(result);
			
			result.Company = await _companyRepository.GetByIdAsync(record.CompanyId);
			
			return result;
		}
	}
}
