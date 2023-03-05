using BlazorForms.Flows.Engine.Fluent;
using BlazorForms.Forms;
using BlazorForms.Shared.Extensions;

namespace MudBlazorUIDemo.Flows.Customer;

public class CustomerListFlow : ListFlowBase<CustomerListFlowModel, FormUserList>
{
    private readonly ICustomerService _customersService;

    public CustomerListFlow(ICustomerService customersService)
    {
        _customersService = customersService;
    }

    public override async Task<CustomerListFlowModel> LoadDataAsync(QueryOptions queryOptions)
    {
        var customers = await _customersService.GetAllCustomersAsync(new CancellationToken());
        var tags = await _customersService.GetAllTags(new CancellationToken());
        return new CustomerListFlowModel
        {
            Customers = customers
                .Select(c => new CustomerFlowModel
                {
                    Customer = c,
                    AllTags = tags.Values.ToList()
                })
                .ToList(),
            
            
        };
    }
}

public class FormUserList : FormListBase<CustomerListFlowModel>
{
    protected override void Define(FormListBuilder<CustomerListFlowModel> builder)
    {
        builder.List(p => p.Customers, e =>
        {
            e.DisplayName = "Customers";

            e.Property(p => p.Customer.Uid).IsPrimaryKey().Label("Uid");
            e.Property(p => p.Customer.Name).Label("Name");
            e.Property(p => p.Customer.Address).Label("Address");

            e.ContextButton("View", "customer-edit/{0}");
            e.NavigationButton("Add", "customer-edit/0");
        });
    }
}