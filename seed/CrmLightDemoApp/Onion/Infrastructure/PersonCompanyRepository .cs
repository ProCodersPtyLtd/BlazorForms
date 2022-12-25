using BlazorForms.Flows;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Domain.Repositories;

namespace CrmLightDemoApp.Onion.Infrastructure
{
    public class PersonCompanyRepository : Repository<PersonCompanyLink>, IPersonCompanyRepository
    {
        private readonly IPersonRepository _personRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IPersonCompanyLinkTypeRepository _personCompanyLinkTypeRepository;

        public PersonCompanyRepository(IPersonRepository personRepository, ICompanyRepository companyRepository,
            IPersonCompanyLinkTypeRepository personCompanyLinkTypeRepository) 
        { 
            _personRepository = personRepository;
            _companyRepository = companyRepository;
            _personCompanyLinkTypeRepository = personCompanyLinkTypeRepository;

            // pre fill some data
            _localCache.Add(new PersonCompanyLink { Id = 1, PersonId = 1, CompanyId = 2, LinkTypeId = 1 });
            _localCache.Add(new PersonCompanyLink { Id = 1, PersonId = 3, CompanyId = 2, LinkTypeId = 4 });
            _id = 10;
        }

        public async Task<List<PersonCompanyLinkDetails>> GetByPersonIdAsync(int personId)
        {
            var list = _localCache.Where(x => !x.Deleted && x.PersonId == personId).Select(x => 
            {
                var item = new PersonCompanyLinkDetails();
                x.ReflectionCopyTo(item);
                return item;
            }).ToList();

            var person = await _personRepository.GetByIdAsync(personId);
            var companyIds = list.Select(x => x.CompanyId).Distinct().ToList();
            var companies = (await _companyRepository.GetListByIdsAsync(companyIds)).ToDictionary(x => x.Id, x => x);
            var linkIds = list.Select(x => x.LinkTypeId).Distinct().ToList();
            var links = (await _personCompanyLinkTypeRepository.GetListByIdsAsync(linkIds)).ToDictionary(x => x.Id, x => x);

            foreach (var item in list)
            {
                item.LinkTypeName = links[item.LinkTypeId].Name;
                item.PersonFullName = $"{person.FirstName} {person.LastName}";
                item.PersonFirstName = person.FirstName;
                item.PersonLastName = person.LastName;
                item.CompanyName = companies[item.CompanyId].Name;
            }

            return list;
        }

        public async Task<List<PersonCompanyLinkDetails>> GetByCompanyIdAsync(int companyId)
        {
            var list = _localCache.Where(x => !x.Deleted && x.CompanyId == companyId).Select(x =>
            {
                var item = new PersonCompanyLinkDetails();
                x.ReflectionCopyTo(item);
                return item;
            }).ToList();

            var company = await _companyRepository.GetByIdAsync(companyId);
            var personIds = list.Select(x => x.PersonId).Distinct().ToList();
            var persons = (await _personRepository.GetListByIdsAsync(personIds)).ToDictionary(x => x.Id, x => x);
            var linkIds = list.Select(x => x.LinkTypeId).Distinct().ToList();
            var links = (await _personCompanyLinkTypeRepository.GetListByIdsAsync(linkIds)).ToDictionary(x => x.Id, x => x);

            foreach (var item in list)
            {
                item.LinkTypeName = links[item.LinkTypeId].Name;
                item.PersonFullName = $"{persons[item.PersonId].FirstName} {persons[item.PersonId].LastName}";
                item.PersonFirstName = persons[item.PersonId].FirstName;
                item.PersonLastName = persons[item.PersonId].LastName;
                item.CompanyName = company.Name;
            }

            return list;
        }
    }
}
