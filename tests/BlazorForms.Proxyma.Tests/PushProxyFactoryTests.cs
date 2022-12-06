using Castle.DynamicProxy;
using BlazorForms.Proxyma;
using BlazorForms.Proxyma.Tests.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BlazorForms.Proxyma.Tests
{
    public class PushProxyFactoryTests
    {
        public class TestProxyScopeConfiguration : IProxyScopeConfiguration
        {
            public IEnumerable<Type> GetScopeTypes()
            {
                var types = new Type[]
                {
                    typeof(Model1),
                    typeof(Addr),
                };

                return types;
            }

            public string GetProxyEngineType()
            {
                return PushProxymaProvider.ProxyEngineType;
            }
        }

        [Fact]
        public void PushProxyConvertModelHierarchyToProxyTest()
        {
            var factory = new ModelProxyFactoryProvider(new TestProxyScopeConfiguration(), new ProxyGenerator());

            var interceptor = factory.CreateModelProxyInterceptor((j, p, o) =>
            {
            });

            var model = new Model1
            {
                ClientId = 3,
                Title = "Mr",
                ResidentialAddress = new Addr { Country = "NZ" },
                AddrArray = new Addr[] { new Addr { Country = "US" }, new Addr { Country = "AU" } },
                AddrList = new List<Addr>(new Addr[] { new Addr { Country = "RU" }, new Addr { Country = "UZ" } }),
            };

            var proxy = factory.GetModelProxy(model, (j, p, o) =>
            {
            }) as Model1;

            var modelProxyType = proxy.GetType();
            Assert.Equal($"{model.GetType().Name}Proxy", modelProxyType.Name);
            Assert.Equal($"{typeof(Addr).Name}Proxy", proxy.ResidentialAddress.GetType().Name);
            Assert.Equal($"{typeof(Addr).Name}Proxy", proxy.AddrArray[0].GetType().Name);
            Assert.Equal($"{typeof(Addr).Name}Proxy", proxy.AddrArray[1].GetType().Name);
            Assert.Equal($"{typeof(Addr).Name}Proxy", proxy.AddrList[0].GetType().Name);
            Assert.Equal($"{typeof(Addr).Name}Proxy", proxy.AddrList[1].GetType().Name);
            Assert.Equal("AU", proxy.AddrArray[1].Country);
            Assert.Equal(0, (proxy.AddrArray[0] as IProxyPropertyBagStore).PropertyBag.Index);
            Assert.Equal(1, (proxy.AddrArray[1] as IProxyPropertyBagStore).PropertyBag.Index);
            Assert.Equal(0, (proxy.AddrList[0] as IProxyPropertyBagStore).PropertyBag.Index);
            Assert.Equal(1, (proxy.AddrList[1] as IProxyPropertyBagStore).PropertyBag.Index);
            Assert.Equal("UZ", proxy.AddrList[1].Country);
        }
    }
}
