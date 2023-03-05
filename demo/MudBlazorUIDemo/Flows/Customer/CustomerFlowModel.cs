using BlazorForms.Flows.Definitions;

namespace MudBlazorUIDemo.Flows.Customer;

public class CustomerListFlowModel : IFlowModel
{
    public IList<CustomerFlowModel> Customers { get; set; }
}

public class CustomerFlowModel : IFlowModel
{
    public CustomerType Customer { get; set; }
    public IList<CustomerTypeTag> AllTags { get; set; } 
}

public record CustomerType
{
    public string Uid { get; init; }
    public string Name { get; init; }
    public string Address { get; init; }
    public IList<CustomerTypeTag> CustomerTags { get; set; }
}


public record CustomerTypeTag
{
    public string Uid { get; init; }
    public string TagName { get; init; }
}
