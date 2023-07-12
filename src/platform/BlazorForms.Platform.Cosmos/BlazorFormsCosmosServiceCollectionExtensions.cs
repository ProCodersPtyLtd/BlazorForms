using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.DependencyInjection;
using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BlazorForms.Platform.Cosmos;

namespace BlazorForms
{
    public static class BlazorFormsCosmosServiceCollectionExtensions
    {

        public static IServiceCollection AddBlazorFormsCosmos([NotNull] this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton(typeof(IFlowRepository), typeof(CosmosFlowRepository))
            ;
            return serviceCollection;
        }

        public static async IAsyncEnumerable<TResult> AsAsyncEnumerable<T, TResult>(this IDocumentQuery<T> query, Func<IEnumerable<T>, IEnumerable<TResult>> processor)
        {
            while (query.HasMoreResults)
            {
                var page = await query.ExecuteNextAsync<T>();
                foreach (var item in processor(page))
                {
                    yield return item;
                }
            }
        }

        public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(this IDocumentQuery<T> query)
        {
            while (query.HasMoreResults)
            {
                var page = await query.ExecuteNextAsync<T>();
                foreach (var item in page)
                {
                    yield return item;
                }
            }
        }
    }    
}
