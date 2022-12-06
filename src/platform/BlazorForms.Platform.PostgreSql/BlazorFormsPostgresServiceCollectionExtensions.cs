using Microsoft.Extensions.DependencyInjection;
using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace BlazorForms.Platform
{
    public static class BlazorFormsPostgresServiceCollectionExtensions
    {

        public static IServiceCollection AddBlazorFormsPostgres([NotNullAttribute] this IServiceCollection serviceCollection)
        {
            // ToDo: commented out for flowRunId removal refactoring
            // serviceCollection.AddSingleton(typeof(IFlowRepository), typeof(NpgsqlFlowRepository));
            return serviceCollection;
        }
    }
}
