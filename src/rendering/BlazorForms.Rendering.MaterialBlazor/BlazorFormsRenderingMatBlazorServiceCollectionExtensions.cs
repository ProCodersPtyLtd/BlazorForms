using MatBlazor;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Reflection;

namespace BlazorForms
{
    public static class BlazorFormsRenderingMatBlazorServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazorFormsMaterialBlazor([NotNull] this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMatBlazor();

            serviceCollection.AddMatToaster(config =>
                {
                    config.Position = MatToastPosition.TopFullWidth;
                    config.PreventDuplicates = true;
                    config.NewestOnTop = false;
                    config.ShowCloseButton = true;
                    config.MaximumOpacity = 95;
                    config.VisibleStateDuration = 10000;
                })
            ;

            return serviceCollection;
        }
    }
}
