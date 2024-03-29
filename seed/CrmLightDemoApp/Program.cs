using Azure.Extensions.AspNetCore.Configuration.Secrets;
using BlazorForms;
using BlazorForms.Flows.Definitions;
using BlazorForms.Platform;
using CrmLightDemoApp;
using CrmLightDemoApp.Onion;
using CrmLightDemoApp.Onion.Domain.Entities;
using CrmLightDemoApp.Onion.Services.Flow;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ConfigureAzureKeyVaultConfiguration();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Register Onion dependencies
builder.Services.AddOnionDependencies();

// MudBlazor
builder.Services.AddMudServices();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopStart;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

// BlazorForms
builder.Services.AddServerSideBlazorForms(new BlazorFormsConfiguration { RuleEngineType = RuleEngineType.Simple });
builder.Services.AddBlazorFormsMudBlazorUI();
builder.Services.AddBlazorFormsServerModelAssemblyTypes(typeof(PersonEditFlow));
builder.Services.AddAdminBusinessObjects();
builder.Services.AddBlazorFormsRenderingFlows();

// Docker
builder.Services.AddAzureDataProtection(builder.Configuration);
builder.Services.AddHealthChecks();

// generics
//builder.Services.AddGenericType(typeof(StaticTypeEditFlow<LeadSourceType>));
//builder.Services.AddGenericType(typeof(FormStaticTypeEdit<LeadSourceType>));
//builder.Services.AddGenericType(typeof(FormStaticTypeSaved<LeadSourceType>));
//builder.Services.AddGenericType(typeof(FormStaticType_ItemChangedRule<LeadSourceType>));
//builder.Services.AddGenericType(typeof(FormStaticType_ItemDeletingRule<LeadSourceType>));
builder.Services.AddGenericTypes(
    new Type[] 
    { 
        typeof(StaticTypeEditFlow<>), 
        typeof(FormStaticTypeEdit<>), 
        typeof(FormStaticTypeSaved<>) ,
        typeof(FormStaticType_ItemChangedRule<>), 
        typeof(FormStaticType_ItemDeletingRule<>), 
    }, new Type[] { typeof(LeadSourceType), typeof(PersonCompanyLinkType) });

var app = builder.Build();

// BlazorForms
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
app.MapHealthChecks("/healthz");

app.Run();

