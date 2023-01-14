using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain.Repositories;
using CrmLightDemoApp.Onion.Infrastructure;
using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services.Flow
{
    public class ClientCompanyEditFlow : FluentFlowBase<ClientCompanyModel>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IClientCompanyRepository _clientCompanyRepository;

        public ClientCompanyEditFlow(ICompanyRepository companyRepository, IPersonRepository personRepository,
            IClientCompanyRepository clientCompanyRepository)
        {
            _companyRepository = companyRepository;
            _personRepository = personRepository;
            _clientCompanyRepository = clientCompanyRepository;
        }

        public override void Define()
        {
            this
                .If(() => _flowContext.Params.ItemKeyAboveZero)
                   .Begin(LoadData)
                   .NextForm(typeof(FormClientCompanyView))
                .EndIf()
                .If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.DeleteButtonBinding)
                    .Next(DeleteData)
                .Else()
                    .If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.EditButtonBinding || !_flowContext.Params.ItemKeyAboveZero)
                        .NextForm(typeof(FormClientCompanyEdit))
                        .Next(SaveData)
                    .EndIf()
                .EndIf()
                .End();
        }

        public async Task LoadData()
        {
            var item = await _clientCompanyRepository.GetByIdAsync(_flowContext.Params.ItemKey);
            // item and Model have different types - we use reflection to copy similar properties
            item.ReflectionCopyTo(Model);

            var persons = (await _personRepository.GetAllAsync())
                .Select(x =>
                {
                    var item = new PersonModel();
                    x.ReflectionCopyTo(item);
                    item.FullName = $"{x.FirstName} {x.LastName}";
                    return item;
                }).OrderBy(x => x.FullName).ToList();

            Model.AllPersons = persons;
            Model.AllCompanies = await _companyRepository.GetAllAsync();
        }

        public async Task DeleteData()
        {
            await _clientCompanyRepository.SoftDeleteAsync(Model);
        }

        public async Task SaveData()
        {
            if (_flowContext.Params.ItemKeyAboveZero)
            {
                await _clientCompanyRepository.UpdateAsync(Model);
            }
            else
            {
                Model.Id = await _clientCompanyRepository.CreateAsync(Model);
            }
        }
    }

    public class FormClientCompanyView : FormEditBase<ClientCompanyModel>
    {
        protected override void Define(FormEntityTypeBuilder<ClientCompanyModel> f)
        {
            f.DisplayName = "Client Company View";

            f.Property(p => p.CompanyId).DropdownSearch(p => p.AllCompanies, m => m.Id, m => m.Name).Label("Company").IsReadOnly();
            f.Property(p => p.ClientManagerId).DropdownSearch(p => p.AllPersons, m => m.Id, m => m.FullName).Label("Manager").IsReadOnly();
            f.Property(p => p.AlternativeClientManagerId).DropdownSearch(p => p.AllPersons, m => m.Id, m => m.FullName).Label("Alternative manager").IsReadOnly();
            f.Property(p => p.StartContractDate).Label("Contract date").Format("dd/MM/yyyy").IsReadOnly();

            f.Button(ButtonActionTypes.Close, "Close");

            f.Button(ButtonActionTypes.Delete, "Delete")
                .Confirm(ConfirmType.Delete, "Delete this Company?", ConfirmButtons.YesNo);

            f.Button(ButtonActionTypes.Edit, "Edit");

        }
    }

    public class FormClientCompanyEdit : FormEditBase<ClientCompanyModel>
    {
        protected override void Define(FormEntityTypeBuilder<ClientCompanyModel> f)
        {

            f.DisplayName = "Client Company Edit";
            f.Confirm(ConfirmType.ChangesWillBeLost, "If you leave before saving, your changes will be lost.", ConfirmButtons.OkCancel);

            f.Property(p => p.CompanyId).DropdownSearch(p => p.AllCompanies, m => m.Id, m => m.Name).Label("Company").IsRequired();
            f.Property(p => p.ClientManagerId).DropdownSearch(p => p.AllPersons, m => m.Id, m => m.FullName).Label("Manager");
            f.Property(p => p.AlternativeClientManagerId).DropdownSearch(p => p.AllPersons, m => m.Id, m => m.FullName).Label("Alternative manager");
            f.Property(p => p.StartContractDate).Label("Contract date").Format("dd/MM/yyyy");

            f.Button(ButtonActionTypes.Cancel, "Cancel");
            f.Button(ButtonActionTypes.Submit, "Save");
        }
    }

    //public class FormCompanyEdit_ItemDeletingRule : FlowRuleBase<CompanyModel>
    //{
    //    public override string RuleCode => "CMP-1";

    //    public override void Execute(CompanyModel model)
    //    {
    //        // preserve all deleted items
    //        model.PersonCompanyLinksDeleted.Add(model.PersonCompanyLinks[RunParams.RowIndex]);
    //    }
    //}

    //public class FormCompanyEdit_ItemChangedRule : FlowRuleBase<CompanyModel>
    //{
    //    public override string RuleCode => "CMP-2";

    //    public override void Execute(CompanyModel model)
    //    {
    //        model.PersonCompanyLinks[RunParams.RowIndex].Changed = true;

    //        // Example: How to trigger cascading rule in FastRuleEngine
    //        Trigger(RowField(m => m.PersonCompanyLinks, m => m.Changed, RunParams.RowIndex));

    //        // Incorrect example: Don't use SingleField for collections
    //        Trigger(SingleField(m => m.PersonCompanyLinks[RunParams.RowIndex].Changed));
    //    }
    //}
    //public class FormCompanyEdit_CheckNameRule : FlowRuleBase<CompanyModel>
    //{
    //    public override string RuleCode => "CMP-3";

    //    public override void Execute(CompanyModel model)
    //    {
    //        var name = model.PersonCompanyLinks[RunParams.RowIndex].PersonFullName;

    //        //if (!model.PersonDictionary.Keys.Contains(name))
    //        if (!model.AllPersons.Any(p => p.FullName == name))
    //        {
    //            Result.ValidationResult = RuleValidationResult.Error;
    //            Result.ValidationMessage = $"Name '{name}' not found";
    //        }
    //    }
    //}
}
