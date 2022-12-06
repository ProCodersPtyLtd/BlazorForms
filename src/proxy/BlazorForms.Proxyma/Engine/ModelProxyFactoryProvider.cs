using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using BlazorForms.Proxyma;

namespace BlazorForms.Proxyma
{
    public class ModelProxyFactoryProvider : IProxymaProvider
    {
        private readonly IProxymaProvider _proxymaProvider;

        public ModelProxyFactoryProvider(IProxyScopeConfiguration proxyScopeConfiguration, IProxyGenerator proxyGenerator) 
        {
            var hashset = new HashSet<Type>(proxyScopeConfiguration.GetScopeTypes());
            var setting = proxyScopeConfiguration.GetProxyEngineType();

            if(setting == PullProxymaProvider.ProxyEngineType)
            {
                _proxymaProvider = new PullProxymaProvider(proxyGenerator, hashset);
            }

            if (setting == PushProxymaProvider.ProxyEngineType)
            {
                _proxymaProvider = new PushProxymaProvider(proxyGenerator, hashset);
            }
        }

        public object CreateClassProxy(Type classToProxy, object[] constructorArguments, object target, ProxyGenerationOptions options, IAsyncInterceptor[] interceptors)
        {
            return _proxymaProvider.CreateClassProxy(classToProxy, constructorArguments, target, options, interceptors);
        }

        public IProxymaInterceptor CreateModelProxyInterceptor(Action<string, string, object> onModelChanging)
        {
            return _proxymaProvider.CreateModelProxyInterceptor(onModelChanging);
        }

        public T CreateModelProxyObject<T>(T source, IProxymaInterceptor interceptor) where T : class
        {
            return _proxymaProvider.CreateModelProxyObject(source, interceptor);
        }

        public T GetModelProxy<T>(T source, Action<string, string, object> changingProperty) where T : class
        {
            return _proxymaProvider.GetModelProxy(source, changingProperty);
        }

        public T GetProxyModel<T>(T source) where T : class
        {
            return _proxymaProvider.GetProxyModel(source);
        }
    }
}
