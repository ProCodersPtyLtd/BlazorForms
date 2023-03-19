using BlazorForms.Platform.Stubs;
using BlazorForms.Platform;
using System.Diagnostics.CodeAnalysis;
using CrmLightDemoApp.Store.Onion.Services.Flow;
using CrmLightDemoApp.Store.Onion.Services.Abstractions;
using CrmLightDemoApp.Store.Onion.Services;
using BlazorForms.Storage;
using BlazorForms.Storage.InMemory;

namespace CrmLightDemoApp.Store.Onion
{
    public static class OnionServiceCollectionExtensions
    {
        public static IServiceCollection AddOnionDependencies([NotNullAttribute] this IServiceCollection serviceCollection)
        {
            serviceCollection
                // repositories
                .AddSingleton<IHighStore, InMemoryHighStore>()
                //.AddSingleton<IPersonRepository, PersonRepository>()
                //.AddSingleton<ICompanyRepository, CompanyRepository>()
                //.AddSingleton<IPersonCompanyRepository, PersonCompanyLinkRepository>()
                //.AddSingleton<IPersonCompanyLinkTypeRepository, PersonCompanyLinkTypeRepository>()
                //.AddSingleton<IRepository<PersonCompanyLinkType>, PersonCompanyLinkTypeRepository>()
                //.AddSingleton<IRepository<LeadSourceType>, LeadSourceTypeRepository>()
                //.AddSingleton<IBoardCardRepository, BoardCardRepository>()
                //.AddSingleton<IClientCompanyRepository, ClientCompanyRepository>()
                //.AddSingleton<IBoardCardHistoryRepository, BoardCardHistoryRepository>()
                //.AddSingleton<IUserRepository, UserRepository>()
                //.AddSingleton<ITenantAccountRepository, TenantAccountRepository>()
                //.AddSingleton<IUserRoleLinkRepository, UserRoleLinkRepository>()
                //.AddSingleton<IRoleRepository, RoleRepository>()
                //// services
                //.AddScoped<IBoardService, BoardService>()
                //.AddScoped<IUserService, UserService>()
                .AddScoped<INotificationService, NotificationService>()
                .AddScoped<IAppAuthState, MockAppAuthState>()

                //.AddSingleton<StaticTypeEditFlow<LeadSourceType>, StaticTypeEditFlow<LeadSourceType>>()
                ;
            return serviceCollection;
        }
    }
}
