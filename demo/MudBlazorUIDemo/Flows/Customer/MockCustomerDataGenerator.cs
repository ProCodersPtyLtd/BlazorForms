namespace MudBlazorUIDemo.Flows.Customer;

using Bogus;
using System.Collections.Generic;

public static class MockDataGenerator
{
    private static readonly Faker _faker = new Faker();

    public static IEnumerable<CustomerTypeTag> GenerateCustomerTypeTags(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new CustomerTypeTag(
                Uid: GenerateUid(),
                TagName: _faker.Random.Word());
        }
    }

    public static IEnumerable<CustomerType> GenerateCustomerTypes(int count)
    {
        var tags = GenerateCustomerTypeTags(5).ToList();

        for (int i = 0; i < count; i++)
        {
            yield return new CustomerType(
                Uid: GenerateUid(),
                Name: _faker.Company.CompanyName(),
                Address: _faker.Address.FullAddress(),
                CustomerTags: _faker.Random.ListItems(tags, _faker.Random.Number(0, 5)));
        }
    }

    private static string GenerateUid()
    {
        // Assumes the Twitter Snowflake format (maximum length of 8 characters)
        // You can replace this with a similar scalable unique ID generator library.
        return _faker.Random.Uuid().ToString("n").Substring(0, 8);
    }
}