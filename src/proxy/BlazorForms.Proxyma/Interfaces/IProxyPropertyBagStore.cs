using BlazorForms.Proxyma.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Proxyma
{
    public interface IProxyPropertyBagStore
    {
        ProxyPropertyBag PropertyBag { get; }
    }
}
