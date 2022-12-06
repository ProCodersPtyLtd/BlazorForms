using Castle.DynamicProxy;
using System;
using System.Collections.Generic;

namespace BlazorForms.Proxyma
{
    public interface IProxymaProvider
    {
        object CreateClassProxy(Type classToProxy, object[] constructorArguments, object target, ProxyGenerationOptions options, IAsyncInterceptor[] interceptors);
        IProxymaInterceptor CreateModelProxyInterceptor(Action<string, string, object> onModelChanging);
        T CreateModelProxyObject<T>(T source, IProxymaInterceptor interceptor) where T : class;
        T GetModelProxy<T>(T source, Action<string, string, object> changingProperty) where T : class;
        T GetProxyModel<T>(T source) where T : class;
    }
}
