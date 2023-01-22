using BlazorForms.FlowRules;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain.Repositories;
using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services.Flow.LeadBoard
{
    public class LoadIsNewClientCompanyRule : FlowRuleBase<LeadBoardCardModel>
    {
        public override string RuleCode => "BRD-1";

        public override void Execute(LeadBoardCardModel model)
        {
            model.ClientCompany.StartContractDate = DateTime.UtcNow;
            model.IsNewCompany = model.RelatedCompanyId == null;
        }
    }

    public class ClientCompanyExistsRule : FlowRuleAsyncBase<LeadBoardCardModel>
    {
        private readonly IClientCompanyRepository _clientCompanyRepository;
        public override string RuleCode => "BRD-2";

        public ClientCompanyExistsRule(IClientCompanyRepository clientCompanyRepository)
        {
            _clientCompanyRepository = clientCompanyRepository;
        }

        public override async Task Execute(LeadBoardCardModel model)
        {
            var cc = await _clientCompanyRepository.FindByCompanyIdAsync(model.RelatedCompanyId ?? 0);

            if (cc != null)
            {
                // disable change fields
                model.IsNewCompany = false;
                Result.Fields[SingleField(m => m.IsNewCompany)].Disabled = true;
                Result.Fields[SingleField(m => m.RelatedCompanyId)].Disabled = true;

                // load Client Company details
                cc.ReflectionCopyTo(model.ClientCompany);
            }
        }
    }

    public class NewClientCompanyRule : FlowRuleBase<LeadBoardCardModel>
    {
        public override string RuleCode => "BRD-3";

        public override void Execute(LeadBoardCardModel model)
        {
            Result.Fields[SingleField(m => m.RelatedCompanyId)].Visible = !model.IsNewCompany;
            Result.Fields[SingleField(m => m.RelatedCompanyId)].Required = !model.IsNewCompany;

            Result.Fields[SingleField(m => m.Company.Name)].Visible = model.IsNewCompany;
            Result.Fields[SingleField(m => m.Company.RegistrationNumber)].Visible = model.IsNewCompany;
            Result.Fields[SingleField(m => m.Company.EstablishedDate)].Visible = model.IsNewCompany;
        }
    }
    public class FormLeadCard_RefreshSources : FlowRuleAsyncBase<LeadBoardCardModel>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IBoardCardHistoryRepository _boardCardHistoryRepository;

        public override string RuleCode => "BRD-4";

        public FormLeadCard_RefreshSources(ICompanyRepository companyRepository, IPersonRepository personRepository,
            IBoardCardHistoryRepository boardCardHistoryRepository)
        {
            _companyRepository = companyRepository;
            _personRepository = personRepository;
            _boardCardHistoryRepository = boardCardHistoryRepository;
        }

        public override async Task Execute(LeadBoardCardModel model)
        {
            // refresh drop down sources
            model.AllPersons = (await _personRepository.GetAllAsync())
                .Select(x =>
                {
                    var item = new PersonModel();
                    x.ReflectionCopyTo(item);
                    item.FullName = $"{x.FirstName} {x.LastName}";
                    return item;
                }).OrderBy(x => x.FullName).ToList();

            model.AllCompanies = (await _companyRepository.GetAllAsync())
                .Select(x =>
                {
                    var item = new CompanyModel();
                    x.ReflectionCopyTo(item);
                    return item;
                }).OrderBy(x => x.Name).ToList();

            // refresh comments
            if (model.Id > 0)
            {
                model.CardHistory = (await _boardCardHistoryRepository.GetListByCardIdAsync(model.Id))
                    .Select(x =>
                    {
                        var item = new CardHistoryModel();
                        x.ReflectionCopyTo(item);
                        return item;
                    }).ToList();
            }
        }
    }
}
