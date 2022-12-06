using System.Net.Http;
using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Platform;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using BlazorForms.Platform.Shared.Interfaces;
using BlazorForms.Proxyma;

namespace BlazorForms.Platform.Integration.Tests.TestApi
{
    public class WebApiTestBase
    {
        protected static readonly HttpClient _client;
        protected readonly IKnownTypesBinder _knownTypesBinder;
        protected Dictionary<string, Type> _knownTypes;
        protected readonly IProxyScopeConfiguration _proxyScopeConfiguration;

        //protected ServiceProvider _serviceProvider;
        //protected readonly ILogger _logger;
        //protected IServiceCollection _serviceCollection;

        static WebApiTestBase()
        {
            var application = new TestApiApplication();
            _client = application.CreateClient();
        }

        public WebApiTestBase()
        {
            var context = AppPartsRegistrationHelper.RegisterPlatformParts(ref _proxyScopeConfiguration, ref _knownTypesBinder);
            _knownTypes = _knownTypesBinder.KnownTypesDict;

            //_serviceCollection = context.ServiceCollection;
            //_serviceCollection.AddSingleton(typeof(IClientRepository), typeof(ClientRepository));
            //_serviceCollection.AddSingleton(typeof(IClientService), typeof(ClientService));
            //_serviceCollection.AddSingleton(typeof(ICrmDomainService), typeof(CrmDomainService));
            //_serviceCollection.AddSingleton(typeof(IAutoMapperConfiguration), typeof(AutoMapperConfiguration));
            //_serviceCollection.AddTransient<ILogStreamer, MockLogStreamer>();
            //_serviceCollection.AddLogging(logging => {
            //    logging.ClearProviders();
            //    logging.AddConsole();
            //    logging.AddDebug();
            //});

            //var config = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            //_serviceCollection.AddSingleton<IConfiguration>(config);

            //_serviceCollection.AddBlazorFormsCrm().AddBlazorFormsCosmos();

            //_serviceProvider = _serviceCollection.BuildServiceProvider();
            //var logger = _serviceProvider.GetRequiredService<ILoggerFactory>();
            //MethodTimeLogger.Logger = logger.CreateLogger("MethodTimeLogger");
            //_logger = _serviceProvider.GetRequiredService<ILogger<IntegrationWebApiTestBase>>();
            //DataConnection.DefaultSettings = new CrmSettings();
            //_proxyGenerator = new ProxyGenerator();

        }
    }
}
