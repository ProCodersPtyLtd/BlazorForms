using BlazorForms.FlowRules;
using BlazorForms.Shared;
using BlazorForms.Platform.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections.ObjectModel;

namespace BlazorForms.Platform.Settings
{
    public class PlatformApplicationPart : IApplicationPart
    {
        public void Initialize()
        {
        }

        public void Register(RegistrationContext context)
        {
            // register assemblies
            var asms = AssemblyHelper.GetAssemblies("BlazorForms.*.dll").Where(a => a.GetName().Name.StartsWith("BlazorForms"));
            var asmsPlatform = AssemblyHelper.GetAssemblies("BlazorForms.Platform*.dll").Where(a => a.GetName().Name.StartsWith("BlazorForms.Platform") && !a.GetName().Name.StartsWith("MatBlazor"));

            context.RegisteredAssemblies.AddRange(asms);
            context.RegisteredAssemblies.AddRange(asmsPlatform);
            
            // ProxyScopeModelTypes
            var modelTypes = context.RegisteredAssemblies.SelectMany(a => a.GetTypes()).Where(t => t.GetCustomAttributes().Any(attr => attr.GetType() == typeof(ProxyScopeAttribute)));
            context.ProxyScopeModelTypes.AddRange(modelTypes);

            // KnownTypes
            var knownAsms = AppDomain.CurrentDomain.GetAssemblies().ToList();
            knownAsms = knownAsms.Where(a => a.GetName().Name.StartsWith("BlazorForms.") || a.GetName().Name.StartsWith("System.Private.CoreLib")).ToList();
            var knownTypes = knownAsms.SelectMany(a => a.GetTypes()).Where(t => t.FullName.StartsWith("BlazorForms.") || t.FullName.StartsWith("System.")).ToList();
            context.DeserializerKnownTypes.AddRange(knownTypes);
            context.DeserializerKnownTypes.Add(typeof(Dictionary<string, object>));
            context.DeserializerKnownTypes.Add(typeof(Dictionary<string, string>));
            context.DeserializerKnownTypes.Add(typeof(FieldDisplayDetails[]));
            context.DeserializerKnownTypes.Add(typeof(DynamicRecordset));
            context.DeserializerKnownTypes.Add(typeof(Dictionary<string, DynamicRecordset>));
            context.DeserializerKnownTypes.Add(typeof(Dictionary<string, ModelField>));

            context.DeserializerKnownTypes.Add(typeof(Dictionary<string, DisplayDetails>));
            context.DeserializerKnownTypes.Add(typeof(Collection<RuleExecutionResult>));
        }
    }
}
