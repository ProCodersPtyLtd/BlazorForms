using BlazorForms.Flows.Definitions;

namespace MudBlazorUIDemo.Flows.Customer;

public class CustomerListFlowModel : IFlowModel
{
    public virtual IEnumerable<CustomerFlowModel> Customers { get; set; }
}

public class CustomerFlowModel : IFlowModel
{
    public virtual CustomerType Customer { get; set; }
    public virtual IEnumerable<CustomerTypeTag> AllTags { get; set; } 
}

public record CustomerType(string Uid, string Name, string Address, IEnumerable<CustomerTypeTag> CustomerTags);

public record CustomerTypeTag(string Uid, string TagName);