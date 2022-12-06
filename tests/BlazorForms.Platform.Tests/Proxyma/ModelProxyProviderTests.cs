using BlazorForms.Flows.Definitions;
using BlazorForms.Proxyma;
using BlazorForms.Platform.Shared.Attributes;
using BlazorForms.Platform.Shared.Interfaces;
using BlazorForms.Tests.Framework.Core;
using System;
using System.Collections.Generic;
using Xunit;
using BlazorForms.Platform.Tests.Models;

namespace BlazorForms.Proxyma.Tests
{
    public class ModelProxyProviderTests : TestInfraAssemblyRegistratorBase
    {
        public ModelProxyProviderTests() : base()
        {
            _proxyScopeConfiguration = new ProxyConfig(PushProxymaProvider.ProxyEngineType, _proxyScopeConfiguration.GetScopeTypes());
        }

        public class ProxyConfig : IProxyScopeConfiguration
        {
            string _proxyType;
            IEnumerable<Type> _types;
            public ProxyConfig(string proxyType, IEnumerable<Type> types)
            {
                _proxyType = proxyType;
                _types = types;
            }
            public string GetProxyEngineType()
            {
                return _proxyType;
            }

            public IEnumerable<Type> GetScopeTypes()
            {
                return _types;
            }
        }

        [Fact]
        public void ModelProxyProviderCreateTest()
        {
            var provider = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            Assert.NotNull(provider);
        }

        [Fact]
        public void CastleGetProxyObjectTest()
        {
            GetProxyObjectTest();
        }

        private void GetProxyObjectTest()
        {
            var provider = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            var interceptor = provider.CreateModelProxyInterceptor(null);
            var model = new Model2 { Client = new ClientModel { ResidentialAddress = new AddressModel { } } };
            var proxy = provider.CreateModelProxyObject(model, interceptor);

            Assert.NotNull(proxy.Client);
        }

        [Fact]
        public void CastleConvertModelHierarchyToProxyTest()
        {
            ConvertModelHierarchyToProxyTest();
        }

        private void ConvertModelHierarchyToProxyTest()
        {
            var provider = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            var interceptor = provider.CreateModelProxyInterceptor(null);
            var model = new Model2 { Client = new ClientModel { FirstName = "Lilu", ResidentialAddress = new AddressModel { StreetLine1 = "14 Sunset Ave" } } };
            var proxy = provider.GetModelProxy(model, (path, name, obj) =>
            {
            });

            Assert.Equal("Lilu", proxy.Client.FirstName);
            var addrProxyTracker = proxy.Client.ResidentialAddress as IProxyPropertyBagStore;
            Assert.NotNull(addrProxyTracker);
            Assert.Equal("14 Sunset Ave", proxy.Client.ResidentialAddress.StreetLine1);
        }

        [Fact]
        public void CastleConvertModelHierarchyToProxyInterceptorTest()
        {
            ConvertModelHierarchyToProxyInterceptorTest();
        }

        private void ConvertModelHierarchyToProxyInterceptorTest()
        {
            bool hit = false;
            var provider = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            //var interceptor = provider.CreateModelProxyInterceptor((jsonPath, prop, val) => 
            //{
            //    Assert.Equal("$.Client.ResidentialAddress.PostCode", jsonPath);
            //    Assert.Equal("PostCode", prop);
            //    Assert.Equal("2230", val);
            //    hit = true;
            //});

            var model = new Model2 { Client = new ClientModel { FirstName = "Lilu", ResidentialAddress = new AddressModel { StreetLine1 = "14 Sunset Ave" } } };
            var proxy = provider.GetModelProxy(model, (jsonPath, prop, val) =>
            {
                Assert.Equal("$.Client.ResidentialAddress.PostCode", jsonPath);
                Assert.Equal("PostCode", prop);
                Assert.Equal("2230", val);
                hit = true;
            });
            proxy.Client.ResidentialAddress.PostCode = "2230";
            Assert.True(hit);
        }

        [Fact]
        public void CastleConvertProxyHierarchyToModelTest()
        {
            ConvertProxyHierarchyToModelTest();
        }

        private void ConvertProxyHierarchyToModelTest()
        {
            var provider = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            var model = new Model2 { Client = new ClientModel { FirstName = "Lilu", ResidentialAddress = new AddressModel { StreetLine1 = "14 Sunset Ave" } } };
            var proxy = provider.GetModelProxy(model, null);
            proxy.Client.ResidentialAddress.PostCode = "2230";
            proxy.Client.LastName = "Yovovich";

            var convertedModel = provider.GetProxyModel(proxy);
            Assert.Equal("Yovovich", convertedModel.Client.LastName);
            Assert.Equal("Lilu", convertedModel.Client.FirstName);
            Assert.Equal("2230", convertedModel.Client.ResidentialAddress.PostCode);
            Assert.Equal("14 Sunset Ave", convertedModel.Client.ResidentialAddress.StreetLine1);
        }


    }

    [ProxyScope]
    public class Model2 : FlowModelBase
    {
        public virtual ClientModel Client { get; set; }
    }
}
