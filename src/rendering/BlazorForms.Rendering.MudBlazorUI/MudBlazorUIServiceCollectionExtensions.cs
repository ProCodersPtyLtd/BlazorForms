using MudBlazor.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Reflection;
using MudBlazor;
using BlazorForms.Rendering;
using BlazorForms.Rendering.MudBlazorUI;

namespace BlazorForms
{
    public static class MudBlazorUIServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazorFormsMudBlazorUI([NotNull] this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IClientBrowserService, MudBlazorUIClientBrowserService>();

            serviceCollection.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopStart;

                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 10000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });

            return serviceCollection;
        }
    }
}
