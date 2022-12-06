using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;

namespace BlazorForms.Proxyma
{
    public abstract class ProxymaProviderBase : IProxymaProvider
    {
        protected readonly IProxyGenerator _proxyGenerator;
        protected readonly HashSet<Type> _modelTypes;

        protected ProxymaProviderBase(IProxyGenerator proxyGenerator, HashSet<Type> hashSet)
        {
            _proxyGenerator = proxyGenerator;
            _modelTypes = hashSet;
        }

        public object CreateClassProxy(Type classToProxy, object[] constructorArguments, object target, ProxyGenerationOptions options, IAsyncInterceptor[] interceptors)
        {
            return _proxyGenerator.CreateClassProxyWithTarget(classToProxy: classToProxy, constructorArguments: constructorArguments, target: target, options: options, interceptors: interceptors);
        }
        public virtual bool IsProxyScopeType(Type t)
        {
            return _modelTypes.Contains(t);
        }

        public virtual T CreateModelProxyObject<T>(T source, IProxymaInterceptor interceptor) where T : class
        {
            var proxy = _proxyGenerator.CreateClassProxyWithTarget(source.GetType(), source, new IInterceptor[] { interceptor as IInterceptor }) as T;
            return proxy;
        }

        public abstract IProxymaInterceptor CreateModelProxyInterceptor(Action<string, string, object> onModelChanging);

        public abstract T GetModelProxy<T>(T source, Action<string, string, object> changingProperty) where T : class;

        public abstract T GetProxyModel<T>(T source) where T : class;
    }
}
