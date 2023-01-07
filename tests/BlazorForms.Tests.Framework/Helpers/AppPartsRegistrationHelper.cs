using Microsoft.Extensions.DependencyInjection;
using BlazorForms.Platform;
using BlazorForms.Platform.Settings;
using BlazorForms.Platform.Shared.ApplicationParts;
using BlazorForms.Proxyma;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Shared;

namespace BlazorForms.Tests.Framework.Helpers
{
    public static class AppPartsRegistrationHelper
    {
        public static RegistrationContext RegisterPlatformParts(ref IProxyScopeConfiguration scopeConfig, ref IKnownTypesBinder knownTypesBinder)
        {
            var serviceCollection = new ServiceCollection();
            var registrationContext = new RegistrationContext(serviceCollection);
            serviceCollection.AddServerSideBlazorForms();
            var ap = new PlatformApplicationPart();
            ap.Initialize();
            ap.Register(registrationContext);
            PlatformProxyScopeConfiguration.InitializeConfiguration(registrationContext.ProxyScopeModelTypes);
            KnownTypesBinder.InitializeConfiguration(registrationContext.DeserializerKnownTypes.Distinct());
            scopeConfig = new PlatformProxyScopeConfiguration();
            knownTypesBinder = new KnownTypesBinder();
            return registrationContext;
        }
    }
}
