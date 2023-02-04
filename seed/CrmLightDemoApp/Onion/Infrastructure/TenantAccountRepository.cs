using BlazorForms.Flows;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain.Entities;
using CrmLightDemoApp.Onion.Domain.Repositories;

namespace CrmLightDemoApp.Onion.Infrastructure
{
    public class TenantAccountRepository : LocalCacheRepository<TenantAccount>, ITenantAccountRepository
    {
        public TenantAccountRepository()
        {
            // pre fill some data
            _localCache.Add(new TenantAccount { Id = 1, CompanyId = 7 });
            _id = 10;
        }
    }
}
