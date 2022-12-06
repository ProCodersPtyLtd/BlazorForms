using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using BlazorForms.Shared;
using BlazorForms.Platform;
using BlazorForms.Rendering;

namespace BlazorForms.Rendering.RadzenBlazor
{
    public class BlazorFormsRenderingRadzenApplicationPart : IApplicationPart
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
