using Microsoft.AspNetCore.Components;
using BlazorForms.Flows.Definitions;
using BlazorForms.Platform;
using BlazorForms.Platform.Crm.Artel;
using BlazorForms;
using BlazorFormsDemoFlows;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

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
services.AddBlazorFormsApplicationParts("BlazorForms.");           // if you want to use examples from BlazorForms.Example.Link without changing namespace
services.AddBlazorFormsApplicationParts("BlazorFormsDemoRadzenApp");    // put root namespace of your project instead of BlazorApp1
services.AddBlazorFormsApplicationParts("BlazorFormsDemoFlows");    // put root namespace of your project instead of BlazorApp1
services.AddBlazorFormsRadzen();

// to build
//services.AddSingleton<IFlowRepository, MockFlowRepository>();
services.AddSingleton(typeof(IFlowRepository), typeof(SqlFlowRepository));
services.AddSingleton<ITenantedScope, MockTenantedScope>();
services.AddBlazorFormsServerModelAssemblyTypes(typeof(ArtelProjectSettingsModel));
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

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
