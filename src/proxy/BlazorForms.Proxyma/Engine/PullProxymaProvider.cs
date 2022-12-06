using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Proxyma
{
    public class PullProxymaProvider : ProxymaProviderBase
    {
        public static readonly string ProxyEngineType = "pull";

        private readonly Dictionary<int, (object proxy, PullPropertyBagInterceptor interceptor)> _interceptors = new();

        public PullProxymaProvider(IProxyGenerator proxyGenerator, HashSet<Type> modelTypes) : base(proxyGenerator, modelTypes)
        {
        }

        public override IProxymaInterceptor CreateModelProxyInterceptor(Action<string, string, object> changingProperty)
        {
            return new PullPropertyBagInterceptor(_proxyGenerator, _modelTypes, changingProperty);
        }

        public override T GetModelProxy<T>(T source, Action<string, string, object> changingProperty)
        {
            var interceptor = CreateModelProxyInterceptor(changingProperty) as PullPropertyBagInterceptor;
            interceptor._sourceModel = source;
            T result = CreateModelProxyObject(source, interceptor);

            var hash = result.GetHashCode();
            _interceptors[hash] = (result, interceptor);
            return result;
        }

        public override T GetProxyModel<T>(T source)
        {
            var hash = source.GetHashCode();

            var interceptor = _interceptors.TryGetValue(hash, out var r) ? r.interceptor : null;
            var result = interceptor?.RevertBack() as T;
            return result;
        }
    }
}
