using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazorForms.Flows.Definitions;
using BlazorForms.Platform.Stubs;
using BlazorForms.Admin.BusinessObjects.RegisteredFlows;
using BlazorForms;
using BlazorForms.Platform;

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
services.AddBlazorFormsApplicationParts("BlazorFormsDemoBlazorApp");    // put root namespace of your project instead of BlazorApp1
services.AddBlazorFormsApplicationParts("BlazorFormsDemoFlows");    // put root namespace of your project instead of BlazorApp1
services.AddBlazorFormsMaterialBlazor();

// to build
//services.AddSingleton<IFlowRepository, MockFlowRepository>();
services.AddSingleton(typeof(IFlowRepository), typeof(SqlFlowRepository));
services.AddSingleton<ITenantedScope, MockTenantedScope>();
services.AddBlazorFormsServerModelAssemblyTypes(typeof(RegisteredListFlow));
services.AddAdminBusinessObjects();
#endregion

var app = builder.Build();
app.BlazorFormsRun();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
