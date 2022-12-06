using MatBlazor;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Radzen;

namespace BlazorForms
{
    public static class BlazorFormsRenderingRadzenServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazorFormsRadzen([NotNull] this IServiceCollection services)
        {
            services.AddMatBlazor();
            
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<TooltipService>();
            services.AddScoped<ContextMenuService>();
            
            return services;
        }
    }
}
