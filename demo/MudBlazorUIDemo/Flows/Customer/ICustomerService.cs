namespace MudBlazorUIDemo.Flows.Customer;

public interface ICustomerService
{
    Task<CustomerType?> GetByIdAsync(string Uid);
    Task DeleteByIdAsync(string Uid);
    Task<CustomerType?> UpsertAsync(CustomerType customer, CancellationToken cancellationToken);
    Task<IList<CustomerType>> GetAllCustomersAsync(CancellationToken cancellationToken);
    Task<IDictionary<string, CustomerTypeTag>> GetAllTags(CancellationToken cancellationToken);
}