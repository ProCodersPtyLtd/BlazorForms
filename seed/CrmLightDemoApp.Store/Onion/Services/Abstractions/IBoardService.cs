using CrmLightDemoApp.Store.Onion.Services.Model;

namespace CrmLightDemoApp.Store.Onion.Services.Abstractions
{
    public interface IBoardService
    {
        Task<List<LeadBoardCardModel>> GetBoardCardsAsync();
        Task CreatingBoardCardAsync(LeadBoardCardModel card);
        Task DeleteBoardCardAsync(LeadBoardCardModel card);
        Task<int> CreateBoardCardAsync(LeadBoardCardModel card);
        Task UpdateBoardCardAsync(LeadBoardCardModel card);

        Task<int> CreateCompanyAsync(CompanyModel company);
        Task<ClientCompanyModel> FindClientCompanyAsync(int companyId);
        Task<int> CreateClientCompanyAsync(ClientCompanyModel clientCompany);
        Task UpdateClientCompanyAsync(ClientCompanyModel clientCompany);

	}
}
