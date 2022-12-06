using Microsoft.Extensions.DependencyInjection;
using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine;
using BlazorForms.Flows.Engine.StateFlow;
using BlazorForms.Forms;
using BlazorForms.ItemStore;
using BlazorForms.Proxyma;
using BlazorForms.Shared;
using BlazorForms.Shared.Reflection;
using BlazorForms.Platform;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms.Platform.Shared.ApplicationParts;
using BlazorForms.Platform.Shared.Interfaces;
using BlazorForms.Platform.Stubs;
using BlazorForms.Rendering;
using BlazorForms.Rendering.Interfaces;
using BlazorForms.Rendering.State;
using BlazorForms.Rendering.Validation;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Reflection;
using BlazorForms.Shared.FastReflection;

namespace BlazorForms
{
    public static class BlazorFormsServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazorFormsApplicationParts([NotNullAttribute] this IServiceCollection serviceCollection, string nameStartsWith)
        {
            var appPartsManager = new ApplicationPartsManager();
            var context = new RegistrationContext(serviceCollection);
            context.RegisteredAssemblies.Add(typeof(BlazorFormsServiceCollectionExtensions).GetTypeInfo().Assembly);
            appPartsManager.RegisterParts(context, nameStartsWith);

            return serviceCollection;
        }

        public static IServiceCollection AddServerSideBlazorForms([NotNull] this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddScoped<IAuthState, MockAuthState>()
                .AddTransient<ILogStreamer, MockLogStreamer>()

                .AddSingleton(typeof(IFlowParser), typeof(FlowParser))
                .AddSingleton<Castle.DynamicProxy.IProxyGenerator, Castle.DynamicProxy.ProxyGenerator>()
                .AddSingleton<IProxymaProvider, ModelProxyFactoryProvider>()
                .AddSingleton<IUniqueIdGenerator, UniqueIdGenerator>()
                .AddSingleton<IFastReflectionProvider, FastReflectionProvider>()

                // InterceptorBasedRuleEngine keeps state during execution and cannot be shared between scopes
                .AddScoped<IRuleExecutionEngine, InterceptorBasedRuleEngine>()
                // RuleDefinitionParser creates all rules, that can have dependencies on Scoped services
                .AddScoped(typeof(IRuleDefinitionParser), typeof(RuleDefinitionParser))
                .AddScoped<IFlowRunProvider, FlowRunProvider>()
                //.AddScoped<IFlowRunEngine, FlowRunEngine>()
                .AddScoped(typeof(IFormDefinitionParser), typeof(FormDefinitionParser))
                .AddScoped<ICustomConfigProvider, CustomConfigProvider>()
                .AddScoped<ICustomConfigStore, CustomConfigStore>()
                .AddScoped<ICustomModelDataProvider, CustomModelDataProvider>()
                .AddScoped<IItemStoreDataProvider, ItemStoreDataProvider>()
                .AddScoped<IStoreDatabaseDriver, SqlJsonDatabaseDriver>()
                .AddScoped<IFluentFlowRunEngine, FluentFlowRunEngine>()
                .AddScoped<IStateFlowRunEngine, StateFlowRunEngine>()

                .AddSingleton<IUserViewDataResolver, UserViewDataResolver>()
                .AddScoped(typeof(IFlowRunStorage), typeof(FlowRunStorage))
                //.AddSingleton(typeof(IFlowRepository), typeof(SqlFlowRepository))
                .AddSingleton(typeof(IFlowRunIdGenerator), typeof(SqlFlowRunIdGenerator))
                //.AddSingleton(typeof(IFlowRunIdGenerator), typeof(NpgsqlFlowRunIdGenerator)) //Postgres
                //.AddSingleton(typeof(IFlowRepository), typeof(SqlFlowRepository))
                .AddSingleton(typeof(IAssemblyRegistrator), typeof(PlatformAssemblyRegistrator))
                .AddSingleton(typeof(IProxyScopeConfiguration), typeof(PlatformProxyScopeConfiguration))
                .AddSingleton(typeof(IKnownTypesBinder), typeof(KnownTypesBinder))
                .AddSingleton(typeof(IObjectCloner), typeof(ObjectCloner))
                .AddSingleton<IReflectionProvider, ReflectionProvider>()


                // BlazorForms rendering
                .AddSingleton<IJsonPathNavigator, JsonPathNavigator>()
                .AddScoped<IModelNavigator, ModelNavigator>()
                .AddScoped<IModelBindingNavigator, ModelBindingNavigator>()
                .AddScoped<IFormViewModel, FormViewModel>()
                .AddScoped(typeof(IFormViewModel<>), typeof(FormViewModel<>))
                .AddScoped<IListFormViewModel, ListFormViewModel>()
                .AddScoped<IDialogFormViewModel, DialogFormViewModel>()
                .AddScoped<HttpClient>()
                .AddScoped<IDynamicFieldValidator, DynamicFieldValidator>()

                //.AddMatToaster(config =>
                //{
                //    config.Position = MatToastPosition.TopFullWidth;
                //    config.PreventDuplicates = true;
                //    config.NewestOnTop = false;
                //    config.ShowCloseButton = true;
                //    config.MaximumOpacity = 95;
                //    config.VisibleStateDuration = 10000;
                //})
            ;
            return serviceCollection;
        }
    }
}
