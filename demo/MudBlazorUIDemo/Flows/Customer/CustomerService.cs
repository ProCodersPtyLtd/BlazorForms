using System.Collections.Concurrent;

namespace MudBlazorUIDemo.Flows.Customer;

class CustomerService : ICustomerService
{
    private readonly ConcurrentDictionary<string, CustomerType> _mockCustomers;
    private readonly ConcurrentDictionary<string, CustomerTypeTag> _mockTags;

    public CustomerService()
    {
        var tags = MockDataGenerator.GenerateCustomerTypeTags(5).ToArray();
        _mockTags = new ConcurrentDictionary<string, CustomerTypeTag>(tags.ToDictionary(k => k.Uid));

        _mockCustomers =
            new ConcurrentDictionary<string, CustomerType>(MockDataGenerator.GenerateCustomerTypes(25, _mockTags.Values.ToList())
                .ToDictionary(k => k.Uid));
    }

    public async Task<CustomerType?> GetByIdAsync(string uid)
    {
        return _mockCustomers.TryGetValue(uid, out var customer) ? customer : default;
    }

    public async Task DeleteByIdAsync(string uid)
    {
        _mockCustomers.TryRemove(uid, out _);
    }

    public async Task<CustomerType?> UpsertAsync(CustomerType customer, CancellationToken cancellationToken)
    {
        var result = _mockCustomers.AddOrUpdate(
            customer?.Uid ?? throw new InvalidOperationException("No Customer record provided"), customer,
            (s, type) => customer);
        
        // update tags
        result.CustomerTags = customer.CustomerTags
            .Select(t => t with { TagName = _mockTags.TryGetValue(t.Uid, out var tag) ? tag.TagName : "<UNKNOWN>" })
            .ToList();

        return result;
    }

    public async Task<IList<CustomerType>> GetAllCustomersAsync(CancellationToken cancellationToken)
    {
        return _mockCustomers.Values.ToList();
    }

    public async Task<IDictionary<string, CustomerTypeTag>> GetAllTags(CancellationToken cancellationToken)
    {
        return _mockTags;
    }
}