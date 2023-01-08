using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Domain.Repositories;
using CrmLightDemoApp.Onion.Services.Abstractions;
using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services
{
    public class BoardService : IBoardService
    {
        private readonly IBoardCardRepository _repo;
        private readonly IPersonRepository _personRepository;
        private readonly IRepository<LeadSourceType> _leadSourceTypeRepository;

        public BoardService(IBoardCardRepository repo, IPersonRepository personRepository, IRepository<LeadSourceType> leadSourceTypeRepository) 
        { 
            _repo = repo;
            _personRepository = personRepository;
            _leadSourceTypeRepository = leadSourceTypeRepository;
        }

        public async Task<int> CreateBoardCardAsync(BoardCardModel card)
        {
            var item = new BoardCard();
            card.ReflectionCopyTo(item);
            card.Id = await _repo.CreateAsync(item);
            return card.Id;
        }

		public async Task CreatingBoardCardAsync(BoardCardModel card)
		{
            card.AllPersons = await GetAllPersons();
            card.AllLeadSources = await GetAllLeadTypes();
		}

		public async Task DeleteBoardCardAsync(BoardCardModel card)
        {
            await _repo.SoftDeleteAsync(card.Id);
        }

        private async Task<List<LeadSourceType>> GetAllLeadTypes()
        {
            return await _leadSourceTypeRepository.GetAllAsync();
		}

        private async Task<List<PersonModel>> GetAllPersons()
        {
            return (await _personRepository.GetAllAsync())
				.Select(x =>
				{
					var item = new PersonModel();
					x.ReflectionCopyTo(item);
					item.FullName = $"{x.FirstName} {x.LastName}";
					return item;
				}).OrderBy(x => x.FullName).ToList();
		}

		public async Task<List<BoardCardModel>> GetBoardCardsAsync()
        {
            var persons = await GetAllPersons();
			var leadTypes = await GetAllLeadTypes();

			var items = (await _repo.GetAllAsync()).Select(x =>
            {
                var item = new BoardCardModel();
                x.ReflectionCopyTo(item);
                item.AllPersons = persons;
                item.AllLeadSources = leadTypes;
                return item;
            }).ToList();

            return items;
        }

        public async Task UpdateBoardCardAsync(BoardCardModel card)
        {
            var item = new BoardCard();
            card.ReflectionCopyTo(item);
            await _repo.UpdateAsync(item);
        }
    }
}
