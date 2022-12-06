using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace BlazorFormsStateFlowDemoApp.BusinessObjects
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices([NotNull] this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddScoped<IDocumentApi, DocumentApi>()
                .AddScoped<DocumentViewModel, DocumentViewModel>()
            ;

            return serviceCollection;
        }

    }
}
