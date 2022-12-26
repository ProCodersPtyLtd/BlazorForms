using BlazorForms;
using BlazorForms.Platform.Crm.Artel;
using BlazorFormsDemoFlows.Flows;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazorUIDemo.Flows;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();


// MudBlazor
var services = builder.Services;
services.AddServerSideBlazorForms();
services.AddBlazorFormsMudBlazorUI();
services.AddBlazorFormsServerModelAssemblyTypes(typeof(ArtelProjectSettingsModel));
services.AddBlazorFormsServerModelAssemblyTypes(typeof(SampleListShortFlow));
services.AddBlazorFormsServerModelAssemblyTypes(typeof(SampleListLargeFlow));

var app = builder.Build();

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
