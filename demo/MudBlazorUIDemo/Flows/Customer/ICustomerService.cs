namespace MudBlazorUIDemo.Flows.Customer;

public interface ICustomerService
{
    Task<CustomerType?> GetByIdAsync(string Uid);
    Task DeleteByIdAsync(string Uid);
    Task<CustomerType?> UpsertAsync(CustomerType? modelCustomer, CancellationToken cancellationToken);
    Task<IEnumerable<CustomerType>> GetAllCustomersAsync(CancellationToken cancellationToken);
}