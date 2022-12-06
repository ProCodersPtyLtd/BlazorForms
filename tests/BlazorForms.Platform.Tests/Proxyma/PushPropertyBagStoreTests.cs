using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BlazorForms.Flows.Definitions;
using BlazorForms.Proxyma;
using BlazorForms.Platform.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using BlazorForms.Tests.Framework.Helpers;

namespace BlazorForms.Platform.Tests.Proxyma
{
    public class PushPropertyBagStoreTests
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

        protected IProxyScopeConfiguration _proxyScopeConfiguration;
        protected readonly IKnownTypesBinder _knownTypesBinder;

        public PushPropertyBagStoreTests()
        {
            IProxyScopeConfiguration pc = null;
            AppPartsRegistrationHelper.RegisterPlatformParts(ref pc, ref _knownTypesBinder);
        }


        [Fact]
        public void ConvertModelHierarchyToProxyTest()
        {
            var proxyma = new ModelProxyFactoryProvider(new TestProxyScopeConfiguration(), new ProxyGenerator());

            var model = new Model1
            {
                ClientId = 3,
                Title = "Mr",
                ResidentialAddress = new Addr { Country = "NZ" },
                AddrArray = new Addr[] { new Addr { Country = "US" }, new Addr { Country = "AU" } },
                AddrList = new List<Addr>(new Addr[] { new Addr { Country = "RU" }, new Addr { Country = "UZ" } }),
            };

            var proxy = proxyma.GetModelProxy(model, null) as Model1;

            Assert.Equal(0, (proxy.AddrArray[0] as IProxyPropertyBagStore).PropertyBag.Index);
            Assert.Equal(1, (proxy.AddrArray[1] as IProxyPropertyBagStore).PropertyBag.Index);
            Assert.Equal("AU", proxy.AddrArray[1].Country);

            Assert.Equal(0, (proxy.AddrList[0] as IProxyPropertyBagStore).PropertyBag.Index);
            Assert.Equal(1, (proxy.AddrList[1] as IProxyPropertyBagStore).PropertyBag.Index);
            Assert.Equal("UZ", proxy.AddrList[1].Country);
        }

        [Fact]
        public void ConvertModelHierarchyToProxyJsonPathTest()
        {
            var proxyma = new ModelProxyFactoryProvider(new TestProxyScopeConfiguration(), new ProxyGenerator());

            var model = new Model1
            {
                ClientId = 3,
                Title = "Mr",
                ResidentialAddress = new Addr { Country = "NZ", Suburb = "Beaverley" },
                AddrArray = new Addr[] { new Addr { Country = "US" }, new Addr { Country = "AU" } },
                AddrList = new List<Addr>(new Addr[] { new Addr { Country = "ZB" }, new Addr { Country = "UZ" } }),
            };

            var settings = _knownTypesBinder.JsonSerializerSettings;

            var jsonText = JsonConvert.SerializeObject(model, settings);
            var json = JObject.Parse(jsonText);
            var proxy = proxyma.GetModelProxy(model, null) as Model1;

            var path = (proxy.AddrArray[1] as IProxyPropertyBagStore).PropertyBag.JsonPath;
            JToken acme = json.SelectToken(path);
            var addr = acme.ToObject<Addr>();
            Assert.Equal("AU", addr.Country);

            path = (proxy.AddrList[0] as IProxyPropertyBagStore).PropertyBag.JsonPath;
            acme = json.SelectToken(path);
            addr = acme.ToObject<Addr>();
            Assert.Equal("ZB", addr.Country);

            path = (proxy.ResidentialAddress as IProxyPropertyBagStore).PropertyBag.JsonPath;
            acme = json.SelectToken(path);
            addr = acme.ToObject<Addr>();
            Assert.Equal("NZ", addr.Country);
        }

        [Fact]
        public void ConvertProxyHierarchyBackToModelWithGenericsTest()
        {
            var proxyma = new ModelProxyFactoryProvider(new TestProxyScopeConfiguration(), new ProxyGenerator());

            var model = new Model1
            {
                ClientId = 3,
                Title = "Mr",
                ResidentialAddress = new Addr { Country = "NZ", Suburb = "Beaverley" },
                AddrArray = new Addr[] { new Addr { Country = "US" }, new Addr { Country = "AU" } },
                AddrList = new List<Addr>(new Addr[] { new Addr { Country = "ZB" }, new Addr { Country = "UZ" } }),
            };

            var proxy = proxyma.GetModelProxy(model, null) as Model1;
            Assert.NotNull(proxy as IProxyPropertyBagStore);
            Assert.NotNull(proxy.AddrArray[1] as IProxyPropertyBagStore);
            Assert.NotNull(proxy.AddrList[1] as IProxyPropertyBagStore);
            Assert.NotNull(proxy.ResidentialAddress as IProxyPropertyBagStore);

            var convertedModel = proxyma.GetProxyModel(proxy);
            Assert.NotNull(convertedModel);
            Assert.NotNull(convertedModel.AddrArray[1]);
            Assert.NotNull(convertedModel.AddrList[1]);
            Assert.NotNull(convertedModel.ResidentialAddress);
            Assert.Null(convertedModel as IProxyPropertyBagStore);
            Assert.Null(convertedModel.AddrArray[1] as IProxyPropertyBagStore);
            Assert.Null(convertedModel.AddrList[1] as IProxyPropertyBagStore);
            Assert.Null(convertedModel.ResidentialAddress as IProxyPropertyBagStore);
        }
    }

    public class Model1 : FlowModelBase
    {
        public virtual Addr[] AddrArray { get; set; }
        public virtual List<Addr> AddrList { get; set; }
        public virtual Tuple<string, int, Addr> TripleTuple { get; set; }
        public virtual List<string> StringList { get; set; }
        public virtual string[] StringArray { get; set; }
        public virtual int? ClientId { get; set; }

        public virtual string Title { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual Addr ResidentialAddress { get; set; }
    }

    public class Addr : FlowModelBase
    {
        //public virtual int? AddressId { get; set; }
        public virtual string StreetLine1 { get; set; }
        public virtual string StreetLine2 { get; set; }
        public virtual string Suburb { get; set; }
        public virtual string State { get; set; }
        public virtual string PostCode { get; set; }
        public virtual string Country { get; set; }
    }
}
