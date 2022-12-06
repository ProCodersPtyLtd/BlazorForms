using Microsoft.Extensions.DependencyInjection;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine;
using BlazorForms.Shared;
using BlazorForms.Rendering.Flows;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace BlazorForms
{
    public static class BlazorFormsRenderingFlowsServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazorFormsRenderingFlows([NotNull] this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddScoped<FlowDiagramViewModel, FlowDiagramViewModel>();

            return serviceCollection;
        }
    }
}
