using BlazorForms;
using BlazorFormsDemoFlows.Flows;
using BlazorForms.Platform.Crm.Artel;
using MauiDemoFormsApp.Data;
using Microsoft.Extensions.Logging;

namespace MauiDemoFormsApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<WeatherForecastService>();

            #region BlazorForms
            var services = builder.Services;

            /*services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
            */
            //services.AddControllersWithViews();
            //services.AddRazorPages();

            services.AddServerSideBlazorForms();
            //services.AddBlazorFormsCosmos();
            //services.AddBlazorFormsApplicationParts("BlazorForms.");           // if you want to use examples from BlazorForms.Example.Link without changing namespace
            //services.AddBlazorFormsApplicationParts("BlazorFormsDemoBlazorApp");    // put root namespace of your project instead of BlazorApp1
            //services.AddBlazorFormsApplicationParts("xyz123");    // put root namespace of your project instead of BlazorApp1
            //services.AddBlazorFormsApplicationParts("BlazorFormsDemoFlows");    // put root namespace of your project instead of BlazorApp1
            services.AddBlazorFormsMaterialBlazor();

            // to build
            //services.AddSingleton<IFlowRepository, MockFlowRepository>();
            //services.AddScoped(typeof(ContentLoader), typeof(ContentLoader));
            //services.AddSingleton(typeof(IFlowRepository), typeof(SqlFlowRepository));
            //services.AddSingleton<ITenantedScope, MockTenantedScope>();
            services.AddBlazorFormsServerModelAssemblyTypes(typeof(ArtelProjectSettingsModel));
            services.AddBlazorFormsServerModelAssemblyTypes(typeof(SampleListShortFlow));
            services.AddBlazorFormsRenderingFlows();

            // MudBlazor
            services.AddBlazorFormsMudBlazorUI();
            #endregion

            return builder.Build();
        }
    }
}