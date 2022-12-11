using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazorForms.Flows.Definitions;
using BlazorForms.Platform;
using BlazorForms.Platform.Crm.Artel;
using BlazorForms;
using BlazorFormsDemoBlazorApp.Data;
using BlazorFormsDemoFlows;
using BlazorForms.Rendering.MaterialBlazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

#region BlazorForms
var services = builder.Services;

services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
//services.AddControllersWithViews();
services.AddRazorPages();

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
services.AddBlazorFormsRenderingFlows();
#endregion

var app = builder.Build();
app.BlazorFormsRun();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
