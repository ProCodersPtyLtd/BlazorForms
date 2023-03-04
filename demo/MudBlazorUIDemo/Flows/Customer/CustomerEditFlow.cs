using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;

namespace MudBlazorUIDemo.Flows.Customer;

public class CustomerEditFlow : FluentFlowBase<CustomerFlowModel>
{
    private readonly ICustomerService _customersService;

    public CustomerEditFlow(ICustomerService customersService)
    {
        _customersService = customersService;
    }

    public override void Define()
    {
        this
            .Begin()
            .Next(LoadData)
            .If(() => _flowContext.Params.ItemKeyAboveZero)
                .NextForm(typeof(FormCustomerView))
                .If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.DeleteButtonBinding)
                    .Next(DeleteData)
                    .End()
                .Else().If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.SubmitButtonBinding ||
                                           !_flowContext.Params.ItemKeyAboveZero)
                    .NextForm(typeof(FormCustomerEdit))
                    .Next(SaveData)
                    .End()
                .EndIf()
            .Else()
                .NextForm(typeof(FormCustomerEdit))
                .Next(SaveData)
                .End()
            .EndIf()
            .End();
    }

    public async Task LoadData()
    {
        if (_flowContext.Params.ItemKeyAboveZero)
        {
            var item = await _customersService.GetByIdAsync(_flowContext.Params.ItemKey.ToString());
            Model.Customer = item ?? new CustomerType(Guid.NewGuid().ToString(), "<Enter Name>", "<Enter Address>", new List<CustomerTypeTag>());
        }
        else
        {
            Model.Customer = new CustomerType(Guid.NewGuid().ToString(), "<Enter Name>", "<Enter Address>", new List<CustomerTypeTag>());
        }
    }

    public async Task DeleteData()
    {
        await _customersService.DeleteByIdAsync(Model.Customer.Uid);
    }

    public async Task SaveData()
    {
        Model.Customer = await _customersService.UpsertAsync(Model.Customer, new CancellationToken()) ??
                         throw new ArgumentException("Failed to upsert Customer");
    }
}

public class FormCustomerView : FormEditBase<CustomerFlowModel>
{
    protected override void Define(FormEntityTypeBuilder<CustomerFlowModel> f)
    {
        f.DisplayName = "Customer View";

        f.Property(p => p.Customer.Uid).Label("Uid").IsRequired().IsReadOnly();
        f.Property(p => p.Customer.Name).Label("Name").IsReadOnly();
        f.Property(p => p.Customer.Address).Label("Address").IsReadOnly();
        f.Table(p => p.Customer.CustomerTags, e => 
        {
            e.DisplayName = "Tags";
            e.Property(p => p.TagName).Label("Tag");
        });

        f.Button(ButtonActionTypes.Submit, "Edit");

        f.Button(ButtonActionTypes.Delete, "Delete")
            .Confirm(ConfirmType.Delete, "Delete this Customer?", ConfirmButtons.YesNo);

        f.Button(ButtonActionTypes.Close, "Close");
    }
}

public class FormCustomerEdit : FormEditBase<CustomerFlowModel>
{
    protected override void Define(FormEntityTypeBuilder<CustomerFlowModel> f)
    {

        f.DisplayName = "Customer Edit";
        f.Confirm(ConfirmType.ChangesWillBeLost, "If you leave before saving, your changes will be lost.",
            ConfirmButtons.OkCancel);

        f.Property(p => p.Customer.Uid).Label("Uid").IsRequired().IsReadOnly();
        f.Property(p => p.Customer.Name).Label("Name").IsReadOnly();
        f.Property(p => p.Customer.Address).Label("Address").IsReadOnly();
        f.Table(p => p.Customer.CustomerTags, builder =>
        {
            
            builder.DisplayName = "Tags";
            builder.Property(p => p.Uid).IsReadOnly();
                
            builder.PropertyRoot(p => p.TagName).DropdownSearch(sm => sm.AllTags, m => m.Uid, m => m.TagName).IsRequired().Label("Tag")
                .Rule(typeof(ItemChangedRule), FormRuleTriggers.ItemChanged);

            builder.Button(ButtonActionTypes.Add);
            builder.Button(ButtonActionTypes.Delete);
            
        }).Confirm(ConfirmType.DeleteItem, "Remove this tag?", ConfirmButtons.YesNo);
        
        f.Button(ButtonActionTypes.Submit, "Save");
        f.Button(ButtonActionTypes.Cancel, "Cancel");
    }
    
    public class ItemChangedRule : FlowRuleBase<CustomerFlowModel>
    {
        public override string RuleCode => "CMP-2";

        public override void Execute(CustomerFlowModel model)
        {
            
        }
    }
}