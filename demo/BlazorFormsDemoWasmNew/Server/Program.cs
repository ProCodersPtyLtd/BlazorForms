using Microsoft.AspNetCore.ResponseCompression;
using BlazorForms.Flows.Definitions;
using BlazorForms.Platform.Crm.Artel;
using BlazorForms;
using BlazorFormsDemoFlows;
using BlazorFormsDemoWasmNew.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

#region BlazorForms
var services = builder.Services;

services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
services.AddRazorPages();

services.AddServerSideBlazorForms();
//services.AddBlazorFormsCosmos();
services.AddBlazorFormsApplicationParts("BlazorForms.");           // if you want to use examples from BlazorForms.Example.Link without changing namespace
services.AddBlazorFormsApplicationParts("BlazorFormsDemoWasmNew.Server");    // put root namespace of your project instead of BlazorApp1
services.AddBlazorFormsApplicationParts("BlazorFormsDemoFlows");    // put root namespace of your project instead of BlazorApp1

// to build
services.AddSingleton<IFlowRepository, MockFlowRepository>();
services.AddSingleton<ITenantedScope, MockTenantedScope>();
services.AddBlazorFormsModelAssemblyTypes(typeof(SampleModel1));
services.AddBlazorFormsModelAssemblyTypes(typeof(SampleModel2));
services.AddBlazorFormsModelAssemblyTypes(typeof(ArtelProjectSettingsModel));
#endregion

var app = builder.Build();
app.BlazorFormsRun();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.BlazorFormsRun();
app.Run();
