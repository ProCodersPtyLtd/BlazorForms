using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazorForms.Flows.Definitions;
using BlazorForms.Platform.Crm.Artel;
using BlazorForms;
using BlazorFormsDemoFastDesignApp.Data;
using BlazorFormsDemoFlows;

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
services.AddBlazorFormsCosmos();
services.AddBlazorFormsApplicationParts("BlazorForms.");           // if you want to use examples from BlazorForms.Example.Link without changing namespace
services.AddBlazorFormsApplicationParts("BlazorFormsDemoFastDesignApp");    // put root namespace of your project instead of BlazorApp1
services.AddBlazorFormsApplicationParts("BlazorFormsDemoFlows");    // put root namespace of your project instead of BlazorApp1
services.AddBlazorFormsStandard();

// to build
//services.AddSingleton<IFlowRepository, MockFlowRepository>();
services.AddSingleton<ITenantedScope, MockTenantedScope>();
services.AddBlazorFormsServerModelAssemblyTypes(typeof(ArtelProjectSettingsModel));
#endregion

var app = builder.Build();

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
