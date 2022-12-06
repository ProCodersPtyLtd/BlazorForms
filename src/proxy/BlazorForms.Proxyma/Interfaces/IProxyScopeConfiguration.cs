using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Proxyma
{
    public interface IProxyScopeConfiguration
    {
        IEnumerable<Type> GetScopeTypes();
        string GetProxyEngineType();
    }
}
