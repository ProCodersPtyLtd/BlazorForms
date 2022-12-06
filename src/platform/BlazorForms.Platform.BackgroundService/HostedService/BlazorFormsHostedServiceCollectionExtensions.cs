using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using BlazorForms.Platform.BackgroundTasks;
using BlazorForms.Platform.BackgroundTasks.HostedService;

namespace BlazorForms
{
    public static class BlazorFormsHostedServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazorFormsHostedService([NotNullAttribute] this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHostedService<QueuedHostedService>();
            serviceCollection.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            return serviceCollection;
        }
    }
}
