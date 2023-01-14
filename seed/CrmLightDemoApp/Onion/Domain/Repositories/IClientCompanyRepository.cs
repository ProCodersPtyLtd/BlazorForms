namespace CrmLightDemoApp.Onion.Domain.Repositories
{
    public interface IClientCompanyRepository : IRepository<ClientCompany>
    {
        Task<ClientCompany> GetByCompanyIdAsync(int companyId);
        //Task<List<ClientCompanyDetails>> GetAllDetailsAsync(int companyId);
        ContextQuery<ClientCompanyDetails> GetAllDetailsContextQuery();
        Task<List<ClientCompanyDetails>> RunAllDetailsContextQueryAsync(ContextQuery<ClientCompanyDetails> ctx);
    }
}
