using MatBlazor;
using Microsoft.Extensions.DependencyInjection;
using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Reflection;
using BlazorForms.Platform;
using BlazorForms.Rendering;
using BlazorForms.Rendering.Interfaces;
using BlazorForms.Rendering.State;
using BlazorForms.Rendering.Validation;
using BlazorForms.Stubs;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using BlazorForms.Shared.FastReflection;

namespace BlazorForms
{
    public static class BlazorFormsWasmServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazorFormsModelAssemblyTypes([NotNullAttribute] this IServiceCollection serviceCollection, Type anyAssemblyType)
        {
            var assembly = anyAssemblyType.GetTypeInfo().Assembly;
            var types = assembly.GetTypes().ToList();

            types.AddRange(new Type[] {
                typeof(System.Dynamic.ExpandoObject),
                typeof(Dictionary<string, string>),
                typeof(Dictionary<string, DynamicRecordset>)
            });

            types.AddRange(typeof(FlowContext).Assembly.GetTypes());
            types.AddRange(typeof(FlowParamsGeneric).Assembly.GetTypes());
            types.AddRange(typeof(DynamicRecordset).Assembly.GetTypes());
            types.AddRange(typeof(FormDetails).Assembly.GetTypes());
            types.AddRange(typeof(RuleEngineExecutionResult).Assembly.GetTypes());
            types.AddRange(typeof(Platform.Definitions.Model.ErrorModel).Assembly.GetTypes());

            KnownTypesBinder.InitializeConfiguration(types.Distinct());

            return serviceCollection;
        }

        public static IServiceCollection AddBlazorFormsWebAssembly([NotNullAttribute] this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMatBlazor();

            serviceCollection
                // ToDo: skipped until FlowRunId removal is in progress
                .AddScoped<IFlowRunProvider, RestFlowRunProvider>()
                .AddSingleton<IJsonPathNavigator, JsonPathNavigator>()
                .AddSingleton<IModelNavigator, ModelNavigator>()
                .AddSingleton<IModelBindingNavigator, ModelBindingNavigator>()
                .AddScoped<IAuthState, MockAuthState>()
                .AddScoped<IDynamicFieldValidator, DynamicFieldValidator>()
                .AddScoped<IFormViewModel, FormViewModel>()
                .AddScoped(typeof(IFormViewModel<>), typeof(FormViewModel<>))
                .AddScoped<IListFormViewModel, ListFormViewModel>()
                .AddScoped<IDialogFormViewModel, DialogFormViewModel>()
                .AddScoped<IClientBrowserService, ClientBrowserService>()
                .AddScoped<IClientDateService, ClientDateService>()
                .AddSingleton(typeof(IKnownTypesBinder), typeof(KnownTypesBinder))
                .AddSingleton<IReflectionProvider, ReflectionProvider>()
                .AddSingleton<IUserViewDataResolver, UserViewDataResolver>()

                .AddMatToaster(config =>
                {
                    config.Position = MatToastPosition.BottomRight;
                    config.PreventDuplicates = true;
                    config.NewestOnTop = true;
                    config.ShowCloseButton = true;
                    config.MaximumOpacity = 95;
                    config.VisibleStateDuration = 3000;
                })
                ;
            ;
            return serviceCollection;
        }
    }
}
