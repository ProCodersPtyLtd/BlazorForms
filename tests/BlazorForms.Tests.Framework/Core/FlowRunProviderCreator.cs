using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Platform;
using BlazorForms.Platform.Crm.Definitions.Services;
using BlazorForms.Platform.Crm.Domain.Services;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms.Platform.Shared.ApplicationParts;
using BlazorForms.Platform.Shared.Interfaces;
using BlazorForms.Rendering;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Platform.Stubs;
using BlazorForms.ItemStore;
using BlazorForms.Admin.BusinessObjects.Interfaces;
using BlazorForms.Admin.BusinessObjects.Providers;
using BlazorForms.Shared.FastReflection;

namespace BlazorForms.Tests.Framework.Core
{
    public class FlowRunProviderCreator : IAssemblyRegistrator
    {
        private ServiceProvider _serviceProvider;
        private FlowRunStorage _flowRunStorage;
        private IFlowParser _flowParser;
        private IFluentFlowRunEngine _fluentFlowEngine;

        public IEnumerable<Assembly> GetConsideredAssemblies()
        {
            var asms = AssemblyHelper.GetAssemblies("BlazorForms*.dll").Where(a => a.GetName().Name.StartsWith("BlazorForms.Framework."));
            var asmsPlatform = AssemblyHelper.GetAssemblies("BlazorForms.Platform.dll").Where(a => a.GetName().Name.StartsWith("BlazorForms.Platform."));
            var asmsApplication = AssemblyHelper.GetAssemblies("BlazorForms.Application*.dll").Where(a => a.GetName().Name.StartsWith("BlazorForms.Application"));
            var adminAssemblies = AssemblyHelper.GetAssemblies("BlazorForms.Admin*.dll").Where(a => a.GetName().Name.StartsWith("BlazorForms.Admin"));
            return asms.Union(asmsPlatform).Union(asmsApplication).Union(adminAssemblies);
        }

        public IFluentFlowRunEngine FluentFlowRunEngine
        {
            get { return _fluentFlowEngine; }
        }
        public ServiceProvider ServiceProvider
        {
            get { return _serviceProvider; }
        }
        public FlowRunStorage FlowRunStorage
        {
            get { return _flowRunStorage; }
        }

        public IFlowParser FlowParser
        {
            get { return _flowParser; }
        }

        public IFlowRunProvider GetFlowRunProvider()
        {
            var serviceCollection = new ServiceCollection();
            var _registrationContext = new RegistrationContext(serviceCollection);

            serviceCollection.AddAuthorizationCore();
            serviceCollection.AddSingleton(typeof(IAutoMapperConfiguration), typeof(AutoMapperConfiguration));
            serviceCollection.AddSingleton<ITenantedScope, MockTenantedScope>();
            serviceCollection.AddSingleton<IFastReflectionProvider, FastReflectionProvider>();
            serviceCollection.AddSingleton<IKnownTypesBinder, KnownTypesBinder>();

            //serviceCollection.AddSingleton<AuthenticationStateProvider, TestIdentityAuthenticationStateProvider<IdentityUser>>();
            serviceCollection.AddScoped<NavigationManager, MockNavigationManager>();
            serviceCollection.AddScoped<IClientBrowserService, MockClientBrowserService>();

            serviceCollection.AddLogging(logging => {
                logging.ClearProviders();
                //logging.AddConsole();
                //logging.AddDebug();
            });

            var config = new ConfigurationBuilder()
                //.AddJsonFile("appsettings.json")
                .Build();

            serviceCollection.AddSingleton<IConfiguration>(config);
            //serviceCollection.AddTransient<ITimesheetNotification, TimesheetNotification>();
            serviceCollection.AddTransient<ILogStreamer, MockLogStreamer>();
            //serviceCollection.AddTransient<IUploadDomainService, MockUploadDomainService>();
            // for some tests we need to construct  SqlFlowRepository
            serviceCollection.AddScoped<IFlowRepository, MockFlowRepository>();
            serviceCollection.AddScoped<IFlowRunStorage, MockFlowRunStorage>();
            serviceCollection.AddScoped<IObjectCloner, MockObjectCloner>();
            serviceCollection.AddScoped<IAuthState, TestAuthState>();

            serviceCollection.AddScoped<IFlowParser, FlowParser>();
            serviceCollection.AddScoped<IFluentFlowRunEngine, FluentFlowRunEngine>();
            serviceCollection.AddScoped<ICustomConfigProvider, CustomConfigProvider>();
            serviceCollection.AddScoped<ICustomConfigStore, CustomConfigStore>();
            serviceCollection.AddScoped<ICustomModelDataProvider, CustomModelDataProvider>();
            serviceCollection.AddScoped<IItemStoreDataProvider, ItemStoreDataProvider>();
            serviceCollection.AddScoped<IStoreDatabaseDriver, MockStoreDatabaseDriver>();

            serviceCollection.AddScoped<IJsonPathNavigator, JsonPathNavigator>();
            serviceCollection.AddScoped<IModelBindingNavigator, ModelBindingNavigator>();
            // Admin Objects
            serviceCollection.AddScoped(typeof(IFlowDataProvider), typeof(FlowDataProvider));

            // Artel Objects
            serviceCollection.AddScoped(typeof(IArtelProjectService), typeof(ArtelProjectService));

            //if (applicationPart != null)
            //{
            //    applicationPart.Initialize();
            //    applicationPart.Register(_registrationContext);
            //}

            //_registrationContext.ServiceCollection.AddServerSideBlazorForms()
            //    //.AddBlazorFormsCrm()
            //    .AddBlazorFormsHostedService();

            //_registrationContext.ServiceCollection.AddBlazorFormsServerModelAssemblyTypes(typeof(RegisteredListFlow));


            serviceCollection.AddSingleton<IAssemblyRegistrator>(sp => this);

            //var ap = new PlatformApplicationPart();
            //ap.Initialize();
            //ap.Register(_registrationContext);

            //var crm = new CrmApplicationPart();
            //crm.Initialize();
            //crm.Register(_registrationContext);

            //if (authStateType == null)
            //{
            //    serviceCollection.AddSingleton(typeof(IAuthState), typeof(TestAuthState));
            //    serviceCollection.AddScoped(typeof(IAccessService), typeof(MockAccessService));
            //}
            //else
            //{
            //    serviceCollection.AddSingleton(typeof(IAuthState), authStateType);
            //    serviceCollection.AddScoped(typeof(IAccessService), typeof(AccessService));
            //}



            //PlatformProxyScopeConfiguration.InitializeConfiguration(_registrationContext.ProxyScopeModelTypes);
            //KnownTypesBinder.InitializeConfiguration(_registrationContext.DeserializerKnownTypes.Distinct());
            //AutoMapperConfiguration.InitializeMapperConfiguration(_registrationContext.AutoMapperProfiles.Distinct());
            //_proxyScopeConfiguration = new PlatformProxyScopeConfiguration();
            //_knownTypesBinder = new KnownTypesBinder();
            //_proxyGenerator = new ProxyGenerator();

            //serviceCollection.AddBlazorFormsCosmos();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            _serviceProvider = serviceProvider;

            IFlowParser parser = new FlowParser(this, serviceProvider.GetRequiredService<IKnownTypesBinder>());
            _flowParser = parser;
            var formParser = new FormDefinitionParser(serviceProvider);
            var ruleParser = new RuleDefinitionParser(serviceProvider);
            dynamic dynamicParams = new ExpandoObject();
            dynamicParams.Top = 10;
            //var repo = _fixture.FlowRepository;
            var repo = serviceProvider.GetRequiredService<IFlowRepository>();
            var cloner = serviceProvider.GetRequiredService<IObjectCloner>();
            var tenantedScope = serviceProvider.GetRequiredService<ITenantedScope>();
            var storage = new FlowRunStorage(repo, cloner, tenantedScope);
            _flowRunStorage = storage;
            //var proxyProvider = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            var dataResolver = new UserViewDataResolver();
            var customConfigProvider = serviceProvider.GetRequiredService<ICustomConfigProvider>();
            var logger = serviceProvider.GetRequiredService<ILogger<FlowRunProvider>>();
            var fluentEngine = new FluentFlowRunEngine(serviceProvider.GetRequiredService<ILogger<FluentFlowRunEngine>>(), serviceProvider, storage, parser);
            _fluentFlowEngine = fluentEngine;
            var auth = serviceProvider.GetRequiredService<IAuthState>();
            var clientBrowserService = new Moq.Mock<IClientBrowserService>();
            clientBrowserService.Setup(e => e.GetWindowOrigin()).ReturnsAsync("https://localhost");

            var provider = new FlowRunProvider(logger, storage, formParser, ruleParser, parser, this, null, serviceProvider,
                dataResolver, customConfigProvider, auth, fluentEngine, serviceProvider.GetRequiredService<ILogStreamer>(), clientBrowserService.Object);

            return provider;
        }

        public void InjectAssembly(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public void RemoveAssembly(Assembly assembly)
        {
            throw new NotImplementedException();
        }
    }
}
