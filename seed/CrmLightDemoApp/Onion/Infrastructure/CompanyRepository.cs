using BlazorForms.Flows;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Domain.Repositories;

namespace CrmLightDemoApp.Onion.Infrastructure
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public CompanyRepository()
        {
            // pre fill some data
            _localCache.Add(new Company { Id = 1, Name = "Mizeratti", RegistrationNumber = "99899632221", EstablishedDate = new DateTime(1908, 1, 17) });
            _localCache.Add(new Company { Id = 2, Name = "Alpha Pajero", RegistrationNumber = "89963222172", EstablishedDate = new DateTime(1956, 5, 14) });
            _id = 10;
        }
    }
}
