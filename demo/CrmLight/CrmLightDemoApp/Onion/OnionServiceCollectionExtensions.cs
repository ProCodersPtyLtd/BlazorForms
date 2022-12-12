using BlazorForms.Platform.Stubs;
using BlazorForms.Platform;
using System.Diagnostics.CodeAnalysis;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Infrastructure;

namespace CrmLightDemoApp.Onion
{
    public static class OnionServiceCollectionExtensions
    {
        public static IServiceCollection AddOnionDependencies([NotNullAttribute] this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddScoped<IPersonRepository, PersonRepository>()
                ;
            return serviceCollection;
        }
    }
}
