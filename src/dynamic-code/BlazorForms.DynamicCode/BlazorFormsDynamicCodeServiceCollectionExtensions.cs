using Microsoft.Extensions.DependencyInjection;
using BlazorForms.DynamicCode;
using BlazorForms.DynamicCode.Engine;
using BlazorForms.DynamicCode.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace BlazorForms
{
    public static class BlazorFormsDynamicCodeServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazorFormsDynamicCode([NotNull] this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<IDynamicFlowRepository, SqlDynamicFlowRepository>()
                .AddSingleton<IDynamicCodeValidationEngine, DynamicCodeValidationEngine>()
                .AddSingleton<IDynamicFlowProvider, DynamicFlowProvider>()
                ;

            return serviceCollection;
        }

    }
}
