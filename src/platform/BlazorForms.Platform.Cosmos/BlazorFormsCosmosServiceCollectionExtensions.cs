using Microsoft.Extensions.DependencyInjection;
using BlazorForms.Flows.Definitions;
using System.Diagnostics.CodeAnalysis;
using BlazorForms.Platform.Cosmos;

namespace BlazorForms
{
    public static class BlazorFormsCosmosServiceCollectionExtensions
    {

        public static IServiceCollection AddBlazorFormsCosmos([NotNull] this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<BfCosmosSerializer>()
                .AddSingleton<IFlowRepository, CosmosFlowRepository>();
            
            return serviceCollection;
        }
    }    
}
