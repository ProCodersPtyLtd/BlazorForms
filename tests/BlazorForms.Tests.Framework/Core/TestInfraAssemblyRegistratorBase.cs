using Castle.DynamicProxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BlazorForms.Shared;
using BlazorForms.Platform.Settings;
using BlazorForms.Platform.Shared.ApplicationParts;
using BlazorForms.Platform.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine;
using BlazorForms.Platform;
using BlazorForms;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms.Rendering;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Proxyma;

namespace BlazorForms.Tests.Framework.Core
{
    public class TestInfraAssemblyRegistratorBase : IAssemblyRegistrator
    {
        public IEnumerable<Assembly> GetConsideredAssemblies()
        {
            var asms = AssemblyHelper.GetAssemblies("BlazorForms*.dll").Where(a => a.GetName().Name.StartsWith("BlazorForms."));
            var asmsPlatform = AssemblyHelper.GetAssemblies("BlazorForms.Platform.dll").Where(a => a.GetName().Name.StartsWith("BlazorForms.Platform."));
            return asms.Union(asmsPlatform);
        }

        public void InjectAssembly(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public void RemoveAssembly(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        protected readonly IServiceCollection _serviceCollection;
        protected readonly RegistrationContext _registrationContext;
        protected IProxyScopeConfiguration _proxyScopeConfiguration;
        protected readonly IKnownTypesBinder _knownTypesBinder;
        protected readonly IProxyGenerator _proxyGenerator;

        protected readonly ServiceProvider _serviceProvider;
        protected readonly ILogger _logger;

        public TestInfraAssemblyRegistratorBase(): this(null)
        { }

        public TestInfraAssemblyRegistratorBase(IApplicationPart applicationPart, Type authStateType = null)
        {
            _serviceCollection = new ServiceCollection();

            _registrationContext = new RegistrationContext(_serviceCollection);

            _serviceCollection.AddAuthorizationCore();
            _serviceCollection.AddSingleton(typeof(IAutoMapperConfiguration), typeof(AutoMapperConfiguration));

            //_serviceCollection.AddSingleton<AuthenticationStateProvider, TestIdentityAuthenticationStateProvider<IdentityUser>>();
            _serviceCollection.AddScoped<NavigationManager, MockNavigationManager>();
            _serviceCollection.AddScoped<IClientBrowserService, MockClientBrowserService>();

            _serviceCollection.AddLogging(logging => {
                logging.ClearProviders();
                //logging.AddConsole();
                //logging.AddDebug();
            });

            var config = new ConfigurationBuilder()
                //.AddJsonFile("appsettings.json")
                .Build();

            _serviceCollection.AddSingleton<IConfiguration>(config);
            //_serviceCollection.AddTransient<ITimesheetNotification, TimesheetNotification>();
            _serviceCollection.AddTransient<ILogStreamer, MockLogStreamer>();
            //_serviceCollection.AddTransient<IUploadDomainService, MockUploadDomainService>();
            // for some tests we need to construct  SqlFlowRepository
            //_serviceCollection.AddScoped<IFlowRepository, CosmosFlowRepository>();
            _serviceCollection.AddScoped<IFlowRepository, MockFlowRepository>();

            if (applicationPart != null)
            {
                applicationPart.Initialize();
                applicationPart.Register(_registrationContext);
            }

            //_registrationContext.ServiceCollection.AddServerSideBlazorForms().AddBlazorFormsCrm().AddBlazorFormsHostedService();

            _serviceCollection.AddSingleton<IAssemblyRegistrator>(sp => this);

            var ap = new PlatformApplicationPart();
            ap.Initialize();
            ap.Register(_registrationContext);

            //var crm = new CrmApplicationPart();
            //crm.Initialize();
            //crm.Register(_registrationContext);

            //if (authStateType == null)
            {
                _serviceCollection.AddSingleton(typeof(IAuthState), typeof(TestAuthState));
                //_serviceCollection.AddScoped(typeof(IAccessService), typeof(MockAccessService));
            }
            //else
            //{
            //    _serviceCollection.AddSingleton(typeof(IAuthState), authStateType);
            //    _serviceCollection.AddScoped(typeof(IAccessService), typeof(AccessService));
            //}



            _registrationContext.ProxyScopeModelTypes.Add(typeof(Money));
            PlatformProxyScopeConfiguration.InitializeConfiguration(_registrationContext.ProxyScopeModelTypes);
            KnownTypesBinder.InitializeConfiguration(_registrationContext.DeserializerKnownTypes.Distinct());
            AutoMapperConfiguration.InitializeMapperConfiguration(_registrationContext.AutoMapperProfiles.Distinct());
            _proxyScopeConfiguration = new PlatformProxyScopeConfiguration();
            _knownTypesBinder = new KnownTypesBinder();
            _proxyGenerator = new ProxyGenerator();

            //_serviceCollection.AddBlazorFormsCosmos();
            _serviceProvider = _serviceCollection.BuildServiceProvider();

            _logger = _serviceProvider.GetRequiredService<ILogger<TestInfraAssemblyRegistratorBase>>();
        }
    }
}
