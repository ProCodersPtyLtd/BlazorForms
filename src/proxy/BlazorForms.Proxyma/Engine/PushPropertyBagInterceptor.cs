using Castle.DynamicProxy;
using BlazorForms.Proxyma.Model;
using System;
using System.Collections.Generic;

namespace BlazorForms.Proxyma
{
    public class PushPropertyBagInterceptor : IProxymaInterceptor, IModelProxyInterceptor, IInterceptor
    {
        private readonly Dictionary<int, ProxyPropertyBag> _propertyBags = new();
        private readonly Action<string, string, object> _changingProperty;

        public PushPropertyBagInterceptor()
        {

        }

        public PushPropertyBagInterceptor(Action<string, string, object> changingProperty)
        {
            _changingProperty = changingProperty;
        }

        public bool InterceptSetter(IProxyPropertyBagStore obj, string prop, object val)
        {
            string jsonPath = GetJsonPath(obj, prop);
            _changingProperty?.Invoke(jsonPath, prop, val);
            return false;
        }

        public static string GetJsonPath(IProxyPropertyBagStore obj, string prop)
        {
            var path = $"{obj.PropertyBag.JsonPath}.{prop}";
            return path;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name == "GetHashCode")
            {
                invocation.Proceed();
                return;
            }

            if (invocation.Method.DeclaringType == typeof(IProxyPropertyBagStore))
            {
                var hash = invocation.Proxy.GetHashCode();
                _propertyBags.TryAdd(hash, new ProxyPropertyBag());

                if (invocation.Method.Name == "set_PropertyBag")
                {
                    _propertyBags[hash] = invocation.Arguments[0] as ProxyPropertyBag;
                }

                if (invocation.Method.Name == "get_PropertyBag")
                {
                    invocation.ReturnValue = _propertyBags[hash];
                }

                return;
            }

            if (invocation.Method.Name.StartsWith("set_"))
            {
                var prop = invocation.Method.Name.Replace("set_", "");
                var obj = invocation.Proxy as IProxyPropertyBagStore;
                var jsonPath = GetJsonPath(obj, prop);
                _changingProperty?.Invoke(jsonPath, prop, invocation.Arguments[0]);
            }
            invocation.Proceed();
        }
    }
}
