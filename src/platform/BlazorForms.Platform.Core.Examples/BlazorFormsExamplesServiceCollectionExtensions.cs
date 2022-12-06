using Microsoft.Extensions.DependencyInjection;
using Pc.Framework.Libs.Flows;
using System.Diagnostics.CodeAnalysis;

namespace Pc.Platz
{
    public static class PlatzExamplesServiceCollectionExtensions
    {
        // Execute it in startup after Platz registration:
        //      ...
        //      services.AddServerSidePlatz();
        //      services.AddPlatzApplicationParts("BlazorApp1.");
        //      services.AddFluentFlowRunner();
        //      ...
        public static IServiceCollection AddFluentFlowRunner([NotNullAttribute] this IServiceCollection serviceCollection)
        {
            //serviceCollection.AddScoped<IFluentFlowRunEngine, FluentFlowRunner>();
            return serviceCollection;
        }
    }
}
