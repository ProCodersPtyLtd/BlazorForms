using Microsoft.Extensions.DependencyInjection;
using BlazorForms.Shared;
using BlazorForms.Platform.Shared.Interfaces;
using BlazorForms.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorForms.Platform.Shared.ApplicationParts
{
    public class ApplicationPartsManager
    {
        public void RegisterParts(RegistrationContext context, string appPrefix)
        {
            context.ServiceCollection.AddSingleton(typeof(IAutoMapperConfiguration), typeof(AutoMapperConfiguration));

            // read all parts using reflection and execute them
            var assemblies = AssemblyHelper.GetAssemblies("BlazorForms.*.dll").Where(a => a.GetName().Name.StartsWith("BlazorForms."));
            var appAssemblies = AssemblyHelper.GetAssemblies($"{appPrefix}*.dll").Where(a => a.GetName().Name.StartsWith($"{appPrefix}"));
            assemblies = assemblies.Union(appAssemblies);
            
            context.RegisteredAssemblies.AddRange(assemblies);
            var types = assemblies.SelectMany(a => a.GetTypes()).Where(t => t.FullName.StartsWith(appPrefix) || t.FullName.StartsWith("BlazorForms."));
            context.DeserializerKnownTypes.AddRange(types);
            
            // for debug
            // ToDo: remove after nuget packaging completed
            var t2 = types.Where(a => a.FullName.StartsWith("BlazorForms.Example"));
            var t3 = types.Where(a => a.FullName.StartsWith("BlazorForms.Sample"));

            var partTypes = assemblies.SelectMany(a => a.GetTypes()).Where(t => typeof(IApplicationPart).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in partTypes)
            {
                var part = Activator.CreateInstance(type) as IApplicationPart;
                part.Initialize();
                part.Register(context);
            }

            foreach(var knownType in context.DeserializerKnownTypes.Distinct())
            {
                // ToDo: register JsonConverter knownType
            }

            foreach (var proxyModelType in context.ProxyScopeModelTypes.Distinct())
            {
                // ToDo: register proxyModelTypes in Proxima
            }

            // ToDo: all Initialize methods should add range to already populated list - multiple execution is possible
            AutoMapperConfiguration.InitializeMapperConfiguration(context.AutoMapperProfiles.Distinct());
            PlatformAssemblyRegistrator.InitializeConfiguration(context.RegisteredAssemblies.Distinct());
            PlatformProxyScopeConfiguration.InitializeConfiguration(context.ProxyScopeModelTypes.Distinct());

            context.DeserializerKnownTypes.Add(typeof(Dictionary<string, string>));
            var t = context.DeserializerKnownTypes.FirstOrDefault(t => t.FullName == "BlazorForms.Platform.Core.Examples.Rendering.ExampleCustomComponent");
            KnownTypesBinder.InitializeConfiguration(context.DeserializerKnownTypes.Distinct());

        }
    }
}
