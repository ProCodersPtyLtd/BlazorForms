using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using BlazorForms;
using BlazorForms.Platform.Crm.Artel;
using BlazorForms.Wasm.InlineFlows;
using BlazorFormsDemoWasmNew.Client;
using BlazorFormsDemoWasmNew.Client.Flows;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazorFormsWebAssembly();
builder.Services.AddBlazorFormsModelAssemblyTypes(typeof(Program));       // types in Blazor Apllication Client assembly (if you have models there)
builder.Services.AddBlazorFormsModelAssemblyTypes(typeof(SampleModel1));
builder.Services.AddBlazorFormsModelAssemblyTypes(typeof(ArtelProjectSettingsModel));
builder.Services.AddBlazorFormsMaterialBlazor();

#region InlineFlows
builder.Services.AddWasmInlineFlows();
builder.Services.AddWasmInlineFlowsAssemblyTypes(typeof(InlineNavigationFlow1));
#endregion

await builder.Build().RunAsync();
