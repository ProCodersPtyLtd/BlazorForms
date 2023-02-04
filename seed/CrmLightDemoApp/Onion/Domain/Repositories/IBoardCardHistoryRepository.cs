using CrmLightDemoApp.Onion.Domain.Entities;

namespace CrmLightDemoApp.Onion.Domain.Repositories
{
    public interface IBoardCardHistoryRepository : IRepository<BoardCardHistory>
    {
        Task<List<BoardCardHistoryDetails>> GetListByCardIdAsync(int cardId);
    }
}
