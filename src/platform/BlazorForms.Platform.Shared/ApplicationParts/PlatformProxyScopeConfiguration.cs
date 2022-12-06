using BlazorForms.Proxyma;
using BlazorForms.Platform.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BlazorForms.Tests.Framework")]
[assembly: InternalsVisibleTo("BlazorForms.Platform.IntegrationTests")]
namespace BlazorForms.Platform.Shared.ApplicationParts
{
    public class PlatformProxyScopeConfiguration : IProxyScopeConfiguration
    {
        private static object _lock = new object();
        private static IEnumerable<Type> _scopeTypes;

        public IEnumerable<Type> GetScopeTypes()
        {
            return _scopeTypes;
        }
 
        internal static void InitializeConfiguration(IEnumerable<Type> scopeTypes)
        {
            lock (_lock)
            {
                if (_scopeTypes == null)
                {
                    _scopeTypes = scopeTypes;
                }
            }
        }

        public string GetProxyEngineType()
        {
            return PullProxymaProvider.ProxyEngineType;
        }
    }
}
