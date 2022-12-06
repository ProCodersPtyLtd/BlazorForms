using Microsoft.Extensions.DependencyInjection;
using BlazorForms.Flows.Definitions;
using BlazorForms.Admin.BusinessObjects.Interfaces;
using BlazorForms.Admin.BusinessObjects.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace BlazorForms.Platform
{
    public static class AdminBusinessObjectsServiceCollectionExtensions
    {

        public static IServiceCollection AddAdminBusinessObjects([NotNullAttribute] this IServiceCollection services)
        {
            services.AddScoped(typeof(IFlowDataProvider), typeof(FlowDataProvider));
            return services;
        }
    }
}
