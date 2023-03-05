namespace MudBlazorUIDemo.Flows.Customer;

using Bogus;
using System.Collections.Generic;

public static class MockDataGenerator
{
    private static readonly Faker _faker = new Faker();

    public static IEnumerable<CustomerTypeTag> GenerateCustomerTypeTags(int count)
    {
        return _faker.Commerce.Categories(15)
            .ToHashSet()
            .Select(c => new CustomerTypeTag { Uid = GenerateUid(), TagName = c })
            .ToList();
    }

    public static IEnumerable<CustomerType> GenerateCustomerTypes(int count, IList<CustomerTypeTag> tags)
    {
        for (var i = 0; i < count; i++)
        {
            var customerTypeTags = _faker.Random.Shuffle(tags).Take(_faker.Random.Int(1, 3)).ToList();
            yield return new CustomerType{
                Uid = GenerateUid(),
                Name = _faker.Company.CompanyName(),
                Address = _faker.Address.FullAddress(),
                CustomerTags = customerTypeTags
            };
        }
    }

    private static string GenerateUid()
    {
        // Assumes the Twitter Snowflake format (maximum length of 8 characters)
        // You can replace this with a similar scalable unique ID generator library.
        return _faker.Random.Uuid().ToString("n")[..8];
    }
}