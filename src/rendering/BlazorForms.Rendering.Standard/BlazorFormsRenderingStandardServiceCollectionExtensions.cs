using MatBlazor;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Fast.Components.FluentUI;

namespace BlazorForms
{
    public static class BlazorFormsRenderingStandardServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazorFormsStandard([NotNull] this IServiceCollection serviceCollection)
        {
            serviceCollection.AddFluentUIComponents();
            serviceCollection.AddMatBlazor();

            serviceCollection.AddMatToaster(config =>
                {
                    config.Position = MatToastPosition.TopFullWidth;
                    config.PreventDuplicates = true;
                    config.NewestOnTop = false;
                    config.ShowCloseButton = true;
                    config.MaximumOpacity = 95;
                    config.VisibleStateDuration = 10000;
                });

            return serviceCollection;
        }
    }
}
