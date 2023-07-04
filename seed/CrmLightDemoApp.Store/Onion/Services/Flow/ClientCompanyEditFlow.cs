using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Storage;
using CrmLightDemoApp.Store.Onion.Services.Model;

namespace CrmLightDemoApp.Store.Onion.Services.Flow
{
    public class ClientCompanyEditFlow : FluentFlowBase<ClientCompanyModel>
    {
        private readonly IHighStore _store;

        public ClientCompanyEditFlow(IHighStore store)
        {
            _store = store;           
        }

        public override void Define()
        {
            this
                .Begin(LoadData)
                .If(() => _flowContext.Params.ItemKeyAboveZero)
                   .NextForm(typeof(FormClientCompanyView))
                .EndIf()
                .If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.DeleteButtonBinding)
                    .Next(DeleteData)
                .Else()
                    .If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.SubmitButtonBinding || !_flowContext.Params.ItemKeyAboveZero)
                        .NextForm(typeof(FormClientCompanyEdit))
                        .Next(SaveData)
                    .EndIf()
                .EndIf()
                .End();
        }

        public async Task LoadData()
        {
            if (_flowContext.Params.ItemKeyAboveZero)
            {
                Model = await _store.GetByIdAsync<ClientCompanyModel>(_flowContext.Params.ItemKey);

                //var item = await _clientCompanyRepository.GetByIdAsync(_flowContext.Params.ItemKey);
                // item and Model have different types - we use reflection to copy similar properties
                //item.ReflectionCopyTo(Model);
            }

            Model.AllPersons = await _store.GetQuery<PersonModel>().ToListAsync();
            Model.AllCompanies = await _store.GetQuery<CompanyModel>().ToListAsync();

            //var persons = (await _personRepository.GetAllAsync())
            //    .Select(x =>
            //    {
            //        var item = new PersonModel();
            //        x.ReflectionCopyTo(item);
            //        item.FullName = $"{x.FirstName} {x.LastName}";
            //        return item;
            //    }).OrderBy(x => x.FullName).ToList();

            //Model.AllPersons = persons;
            //Model.AllCompanies = await _companyRepository.GetAllAsync();
        }

        public async Task DeleteData()
        {
            await _store.SoftDeleteAsync(Model);
        }

        public async Task SaveData()
        {
            await _store.UpsertAsync(Model);
            //if (_flowContext.Params.ItemKeyAboveZero)
            //{
            //    await _clientCompanyRepository.UpdateAsync(Model);
            //}
            //else
            //{
            //    Model.Id = await _clientCompanyRepository.CreateAsync(Model);
            //}
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

            f.Button(ButtonActionTypes.Submit, "Edit");

            f.Button(ButtonActionTypes.Delete, "Delete")
                .Confirm(ConfirmType.Delete, "Delete this Company?", ConfirmButtons.YesNo);

            f.Button(ButtonActionTypes.Close, "Close");
        }
    }

    public class FormClientCompanyEdit : FormEditBase<ClientCompanyModel>
    {
        protected override void Define(FormEntityTypeBuilder<ClientCompanyModel> f)
        {

            f.DisplayName = "Client Company Edit";
            f.Confirm(ConfirmType.ChangesWillBeLost, "If you leave before saving, your changes will be lost.", ConfirmButtons.OkCancel);

            f.Property(p => p.CompanyId).DropdownSearch(p => p.AllCompanies, m => m.Id, m => m.Name).Label("Company").IsRequired()
                .Rule(typeof(FormClientCompanyEdit_CompanyDupsRule));

            f.Property(p => p.ClientManagerId).DropdownSearch(p => p.AllPersons, m => m.Id, m => m.FullName).Label("Manager");
            f.Property(p => p.AlternativeClientManagerId).DropdownSearch(p => p.AllPersons, m => m.Id, m => m.FullName).Label("Alternative manager");
            f.Property(p => p.StartContractDate).Label("Contract date").Format("dd/MM/yyyy");

            f.Button(ButtonActionTypes.Submit, "Save");
            f.Button(ButtonActionTypes.Cancel, "Cancel");
        }
    }

    public class FormClientCompanyEdit_CompanyDupsRule : FlowRuleAsyncBase<ClientCompanyModel>
    {
        private readonly IHighStore _store;
        public override string RuleCode => "CCE-1";

        public FormClientCompanyEdit_CompanyDupsRule(IHighStore store)
        {
            _store = store;
        }

        public override async Task Execute(ClientCompanyModel model)
        {
            if (model.CompanyId > 0)
            {
                var existing = await _store.GetQuery<ClientCompanyModel>().
                    Where(m => m.CompanyId == model.CompanyId && m.Id != model.Id).FirstOrDefaultAsync();

                if (existing != null && existing.Id != model.Id)
                {
                    Result.ValidationResult = RuleValidationResult.Error;
                    var name = model.AllCompanies.First(x => x.Id == model.CompanyId).Name;
                    Result.ValidationMessage = $"Client for Company '{name}' already exists";
                }
            }
        }
    }
}
