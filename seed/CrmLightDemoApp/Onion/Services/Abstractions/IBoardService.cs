using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services.Abstractions
{
    public interface IBoardService
    {
        Task<List<BoardCardModel>> GetBoardCardsAsync();
        Task CreatingBoardCardAsync(BoardCardModel card);
        Task DeleteBoardCardAsync(BoardCardModel card);
        Task<int> CreateBoardCardAsync(BoardCardModel card);
        Task UpdateBoardCardAsync(BoardCardModel card);
    }
}
