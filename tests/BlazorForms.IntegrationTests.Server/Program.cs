//ToDo: BlazorForms application registration should happen here

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlazorForms.Integration.Tests.Server;
using BlazorForms.Platform.Shared.ApplicationParts;
using BlazorForms.Platform;
using BlazorForms.Shared;
using BlazorForms.Shared.Reflection;
using BlazorForms.Platform.Shared.Interfaces;
//using BlazorForms.Platform.Crm.Domain.Settings;
using BlazorForms.Rendering;
using System.Diagnostics.CodeAnalysis;
using BlazorForms.Flows.Definitions;
using BlazorForms.Proxyma;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms.Flows.Engine;
using BlazorForms.Forms;
using BlazorForms.FlowRules;
using BlazorForms.ItemStore;
using BlazorForms.Flows;
using BlazorForms.Rendering.State;
using System.Net.Http;
using BlazorForms.Rendering.Validation;
using BlazorForms;
using BlazorForms.Platform.Stubs;
using BlazorForms.Platform.Crm.Definitions.Services;
//using BlazorForms.Platform.Crm.Domain.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
//using Xero.NetStandard.OAuth2.Config;
//using Xero.NetStandard.OAuth2.Client;
//using Xero.NetStandard.OAuth2.Api;
using Microsoft.AspNetCore.Identity.UI.Services;
using BlazorFormsDemoWasmNew.Server;
using BlazorForms.Platform.Crm.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using BlazorForms.Admin.BusinessObjects.Interfaces;
using BlazorForms.Admin.BusinessObjects.Providers;
using BlazorForms.Flows.Engine.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Configure

var services = builder.Services;
services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));
services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

services.AddDataProtection();

services.AddBlazorFormsWasmServer();
services.AddServerSideBlazorForms();
//services.AddBlazorFormsCrm();
//services.AddBlazorFormsHostedService();
services.AddSingleton<ITenantedScope, MockTenantedScope>();
services.AddScoped<IArtelProjectService, ArtelProjectService>();
services.AddBlazorFormsApplicationParts("BlazorForms.Integration.Tests.Server");
services.AddBlazorFormsApplicationParts("BlazorForms.");

services
    .AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>()
    .AddSingleton<IJsonPathNavigator, JsonPathNavigator>()
    .AddSingleton<IModelNavigator, ModelNavigator>()
    .AddScoped(typeof(IFlowDataProvider), typeof(FlowDataProvider))
    ;

//services.AddScoped(typeof(IUploadDomainService), typeof(UploadDomainService));

services.AddTransient<ILogStreamer, MockLogStreamer>();

//services.AddTransient<ITimesheetNotification, TimesheetNotification>();
services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

//services.TryAddSingleton(new XeroConfiguration
//{
//    AppName = builder.Configuration["Xero:AppName"],
//    ClientId = builder.Configuration["Xero:ClientId"],
//    ClientSecret = builder.Configuration["Xero:ClientSecret"],
//    CallbackUri = new Uri(builder.Configuration["Xero:CallbackUri"]),
//    Scope = builder.Configuration["Xero:Scope"],
//    State = builder.Configuration["Xero:State"],
//});

//services.TryAddScoped<IXeroClient, XeroClient>();
//services.TryAddScoped<IAccountingApi, AccountingApi>();

RegisterIoC(services);

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

try
{
    app.BlazorFormsRun();
    app.Run();
}
catch (Exception ex)
{
    throw;
}

Console.WriteLine("Test server started");

void RegisterIoC(IServiceCollection services)
{
    Console.WriteLine("RegisterIoC executed");

    var context = new RegistrationContext(services);
    Console.WriteLine("Executing ApplicationPartsManager RegisterParts");
    RegisterParts(context, "BlazorForms.");

    services.AddSingleton(typeof(IAuthState), typeof(TestAuthState));
    services.AddScoped(typeof(IAccessService), typeof(MockAccessService));

    Console.WriteLine("Completed ApplicationPartsManager RegisterParts");
}

void RegisterParts(RegistrationContext context, string appPrefix)
{
    context.ServiceCollection.AddSingleton(typeof(IAutoMapperConfiguration), typeof(AutoMapperConfiguration));

    IApplicationPart part = new WasmServerPart();
    part.Initialize();
    part.Register(context);

    part = new BlazorFormsRenderingApplicationPart();
    part.Initialize();
    part.Register(context);

    //part = new CrmApplicationPart();
    //part.Initialize();
    //part.Register(context);

    foreach (var knownType in context.DeserializerKnownTypes.Distinct())
    {
        // ToDo: register JsonConverter knownType
    }

    foreach (var proxyModelType in context.ProxyScopeModelTypes.Distinct())
    {
        // ToDo: register proxyModelTypes in Proxima
    }

    AutoMapperConfiguration.InitializeMapperConfiguration(context.AutoMapperProfiles.Distinct());
    PlatformAssemblyRegistrator.InitializeConfiguration(context.RegisteredAssemblies.Distinct());
    PlatformProxyScopeConfiguration.InitializeConfiguration(context.ProxyScopeModelTypes.Distinct());

    context.DeserializerKnownTypes.Add(typeof(Dictionary<string, string>));
    KnownTypesBinder.InitializeConfiguration(context.DeserializerKnownTypes.Distinct());

}
public static class BlazorFormsWasmServerServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorFormsWasmServer([NotNullAttribute] this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IAuthState, MockAuthState>()
            .AddTransient<ILogStreamer, MockLogStreamer>()

            .AddSingleton(typeof(IFlowParser), typeof(FlowParser))
            .AddSingleton<Castle.DynamicProxy.IProxyGenerator, Castle.DynamicProxy.ProxyGenerator>()
            .AddSingleton<IProxymaProvider, ModelProxyFactoryProvider>()
            .AddSingleton<IUniqueIdGenerator, UniqueIdGenerator>()
            .AddScoped(typeof(IRuleDefinitionParser), typeof(RuleDefinitionParser))

            .AddScoped<IFlowRunProvider, FlowRunProvider>()
            .AddScoped(typeof(IFormDefinitionParser), typeof(FormDefinitionParser))
            .AddScoped<IRuleExecutionEngine, InterceptorBasedRuleEngine>()
            .AddScoped<ICustomConfigProvider, CustomConfigProvider>()
            .AddScoped<ICustomConfigStore, CustomConfigStore>()
            .AddScoped<ICustomModelDataProvider, CustomModelDataProvider>()
            .AddScoped<IItemStoreDataProvider, ItemStoreDataProvider>()
            .AddScoped<IStoreDatabaseDriver, SqlJsonDatabaseDriver>()
            .AddScoped<IFluentFlowRunEngine, FluentFlowRunEngine>()

            .AddSingleton<IUserViewDataResolver, UserViewDataResolver>()
            .AddScoped(typeof(IFlowRunStorage), typeof(FlowRunStorage))
            .AddSingleton(typeof(IFlowRepository), typeof(SqlFlowRepository))
            .AddSingleton(typeof(IFlowRunIdGenerator), typeof(SqlFlowRunIdGenerator))
            .AddSingleton(typeof(IAssemblyRegistrator), typeof(PlatformAssemblyRegistrator))
            .AddSingleton(typeof(IProxyScopeConfiguration), typeof(PlatformProxyScopeConfiguration))
            .AddSingleton(typeof(IKnownTypesBinder), typeof(KnownTypesBinder))
            .AddSingleton<IReflectionProvider, ReflectionProvider>()
            .AddSingleton<IJsonPathNavigator, JsonPathNavigator>()

            // BlazorForms rendering
            .AddSingleton<IJsonPathNavigator, JsonPathNavigator>()
            .AddSingleton<IModelNavigator, ModelNavigator>()
            .AddScoped<HttpClient>()
            .AddScoped<IDynamicFieldValidator, DynamicFieldValidator>()
            ;

        return serviceCollection;
    }
}

