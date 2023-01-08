using BlazorForms.Flows;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Domain.Repositories;

namespace CrmLightDemoApp.Onion.Infrastructure
{
    public class BoardCardRepository : LocalCacheRepository<BoardCard>, IBoardCardRepository
    {
        public BoardCardRepository()
        {
            // pre fill some data
            _localCache.Add(new BoardCard { Id = 1, Title = "Mizeratti", State = "Lead", LeadSourceTypeId = 1, Description = "Looking for SF dev",
                SalesPersonId = 1, ContactDetails = "https://www.linkedin.com/in/ev-uklad/" });
            
            _id = 10;
        }
    }
}
