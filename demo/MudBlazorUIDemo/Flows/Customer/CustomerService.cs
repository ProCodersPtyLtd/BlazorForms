using System.Collections.Concurrent;

namespace MudBlazorUIDemo.Flows.Customer;

class CustomerService : ICustomerService
{
    private ConcurrentDictionary<string, CustomerType> _mockRepository = new();

    public CustomerService()
    {
        
    }

    public async Task<CustomerType?> GetByIdAsync(string Uid)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteByIdAsync(string Uid)
    {
        throw new NotImplementedException();
    }

    public async Task<CustomerType?> UpsertAsync(CustomerType? modelCustomer, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CustomerType>> GetAllCustomersAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}