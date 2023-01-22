using BlazorForms.Flows;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Domain.Repositories;
using System.ComponentModel.Design;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CrmLightDemoApp.Onion.Infrastructure
{
    public class BoardCardHistoryRepository : LocalCacheRepository<BoardCardHistory>, IBoardCardHistoryRepository
    {
        private readonly IPersonRepository _personRepository;

        public BoardCardHistoryRepository(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
            // pre fill some data
        }

        public async Task<List<BoardCardHistoryDetails>> GetListByCardIdAsync(int cardId)
        {
            var list = _localCache.Where(x => !x.Deleted && x.BoardCardId == cardId).Select(x =>
            {
                var item = new BoardCardHistoryDetails();
                x.ReflectionCopyTo(item);
                return item;
            }).OrderByDescending(x => x.Date).ToList();

            var personIds = list.Select(x => x.PersonId).Distinct().ToList();
            var persons = (await _personRepository.GetListByIdsAsync(personIds)).ToDictionary(x => x.Id, x => x);

            foreach (var item in list)
            {
                item.PersonFullName = $"{persons[item.PersonId].FirstName} {persons[item.PersonId].LastName}";
            }

            return list;
        }
    }
}
