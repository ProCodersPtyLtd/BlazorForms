using Microsoft.Extensions.DependencyInjection;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace BlazorForms.Wasm.InlineFlows
{
    public static class BlazorFormsWasmInlineFlowsServiceCollectionExtensions
    {
        public static IServiceCollection AddWasmInlineFlowsAssemblyTypes([NotNull] this IServiceCollection serviceCollection, Type anyAssemblyType)
        {
            var assemblies = new List<Assembly>();
            assemblies.Add(anyAssemblyType.GetTypeInfo().Assembly);
            assemblies.Add(typeof(FlowContext).Assembly);
            assemblies.Add(typeof(FlowParamsGeneric).Assembly);
            assemblies.Add(typeof(FlowParamsGeneric).Assembly);

            WasmInlineFlowsAssemblyRegistrator.InitializeConfiguration(assemblies.Distinct());

            return serviceCollection;
        }

        public static IServiceCollection AddWasmInlineFlows([NotNull] this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<IAssemblyRegistrator, WasmInlineFlowsAssemblyRegistrator>()
                .AddSingleton<ITenantedScope, MockTenantedScope>()
                .AddSingleton(typeof(IObjectCloner), typeof(MockObjectCloner))
                .AddSingleton(typeof(IFlowParser), typeof(FlowParser))
                .AddSingleton<IFlowRepository, MockFlowRepository>()
                .AddSingleton<IFlowRunStorage, FlowRunStorage>()
                .AddScoped<IFlowRunEngine, FluentFlowRunEngine>()
                .AddScoped<IInlineFlowProvider, InlineFlowProvider>();

            return serviceCollection;
        }
    }
}
