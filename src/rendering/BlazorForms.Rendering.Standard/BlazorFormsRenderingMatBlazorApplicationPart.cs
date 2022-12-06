using Microsoft.Extensions.DependencyInjection;
using BlazorForms.Shared;
using BlazorForms.Shared.Reflection;
using BlazorForms.Platform;
using BlazorForms.Rendering.State;
using BlazorForms.Rendering.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using BlazorForms.Rendering;

namespace BlazorForms.Rendering
{
    public class BlazorFormsRenderingMatBlazorApplicationPart : IApplicationPart
    {
        public void Initialize()
        {
        }

        public void Register(RegistrationContext context)
        {
            // register assemblies
            var asms = AssemblyHelper.GetAssemblies("BlazorForms.Rendering.dll").Where(a => a.GetName().Name.StartsWith("BlazorForms.Rendering") && !a.GetName().Name.StartsWith("MatBlazor")); ;
            context.RegisteredAssemblies.AddRange(asms);

            // KnownTypes
            context.ServiceCollection.AddScoped<IClientBrowserService, ClientBrowserService>();
            context.ServiceCollection.AddScoped<IClientDateService, ClientDateService>();
        }
    }
}
