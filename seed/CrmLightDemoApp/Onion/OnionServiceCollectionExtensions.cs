using BlazorForms.Platform.Stubs;
using BlazorForms.Platform;
using System.Diagnostics.CodeAnalysis;
using CrmLightDemoApp.Onion.Infrastructure;
using CrmLightDemoApp.Onion.Domain.Repositories;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Services.Flow;

namespace CrmLightDemoApp.Onion
{
    public static class OnionServiceCollectionExtensions
    {
        public static IServiceCollection AddOnionDependencies([NotNullAttribute] this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<IPersonRepository, PersonRepository>()
                .AddSingleton<ICompanyRepository, CompanyRepository>()
                .AddSingleton<IPersonCompanyRepository, PersonCompanyRepository>()
                .AddSingleton<IPersonCompanyLinkTypeRepository, PersonCompanyLinkTypeRepository>()
                .AddSingleton<IRepository<PersonCompanyLinkType>, PersonCompanyLinkTypeRepository>()
                .AddSingleton<IRepository<LeadSourceType>, LeadSourceTypeRepository>()

                //.AddSingleton<StaticTypeEditFlow<LeadSourceType>, StaticTypeEditFlow<LeadSourceType>>()
                ;
            return serviceCollection;
        }
    }
}
