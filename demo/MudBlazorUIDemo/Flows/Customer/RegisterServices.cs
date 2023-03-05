using BlazorForms;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MudBlazorUIDemo.Flows.Customer;

public static class RegisterServices
{
    public static IServiceCollection AddCustomerDemoServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddBlazorFormsServerModelAssemblyTypes(typeof(CustomerListFlow));
        services.AddBlazorFormsServerModelAssemblyTypes(typeof(CustomerEditFlow));
        services.TryAddSingleton<ICustomerService, CustomerService>();
        return services;
    }
}