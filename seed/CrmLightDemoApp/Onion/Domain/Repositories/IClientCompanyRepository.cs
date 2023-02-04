using CrmLightDemoApp.Onion.Domain.Entities;

namespace CrmLightDemoApp.Onion.Domain.Repositories
{
    public interface IClientCompanyRepository : IRepository<ClientCompany>
    {
        Task<ClientCompany> FindByCompanyIdAsync(int companyId);
        ContextQuery<ClientCompanyDetails> GetAllDetailsContextQuery();
        Task<List<ClientCompanyDetails>> RunAllDetailsContextQueryAsync(ContextQuery<ClientCompanyDetails> ctx);
    }
}
