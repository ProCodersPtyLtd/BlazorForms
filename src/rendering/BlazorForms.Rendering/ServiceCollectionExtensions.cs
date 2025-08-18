using Microsoft.Extensions.DependencyInjection;
using BlazorForms.Shared;
using BlazorForms.Shared.Reflection;
using BlazorForms.Rendering;
using BlazorForms.Rendering.Interfaces;
using BlazorForms.Rendering.Validation;
using System.Diagnostics.CodeAnalysis;
using BlazorForms.Shared.FastReflection;
using BlazorForms.Rendering.ViewModels;

namespace BlazorForms
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazorFormsRendering([NotNull] this IServiceCollection serviceCollection)
        {
            serviceCollection
                // BlazorForms rendering
                //.AddSingleton<IJsonPathNavigator, JsonPathNavigator>()
                //.AddScoped<IModelNavigator, ModelNavigator>()
                //.AddSingleton<IModelBindingNavigator, ModelBindingNavigator>()
                .AddScoped<IFormViewModel, FormViewModel>()
                .AddScoped(typeof(IFormViewModel<>), typeof(FormViewModel<>))
                .AddScoped<IListFormViewModel, ListFormViewModel>()
                .AddScoped<IDialogFormViewModel, DialogFormViewModel>()
                .AddScoped<BoardDialogViewModel, BoardDialogViewModel>()
                .AddScoped<CardListViewModel, CardListViewModel>()
                .AddScoped<ControlDialogFormViewModel, ControlDialogFormViewModel>()
                .AddScoped<HttpClient>()
                .AddScoped<IDynamicFieldValidator, DynamicFieldValidator>()

                // trying new approach where each page has it's own ViewModel instance
                .AddTransient<IFlowBoardViewModel, FlowBoardViewModel>()
            ;
            return serviceCollection;
        }
    }
}
