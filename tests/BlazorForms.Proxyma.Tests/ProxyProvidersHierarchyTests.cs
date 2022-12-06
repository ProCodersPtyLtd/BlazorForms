using Castle.DynamicProxy;
using Newtonsoft.Json;
using BlazorForms.Proxyma;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Xunit;

namespace BlazorForms.Proxyma.Tests
{
    public class ProxyProvidersHierarchyTests
    {
        [Fact]
        public void ProxyProviderCreateTestPull()
        {
            ProxyProviderCreateTest(new ProxyConfig(PullProxymaProvider.ProxyEngineType));
        }

        [Fact]
        public void ProxyProviderCreateTestPush()
        {
            ProxyProviderCreateTest(new ProxyConfig(PushProxymaProvider.ProxyEngineType));
        }

        internal void ProxyProviderCreateTest(ProxyConfig cfg)
        {
            var model = GetModel();
            var provider = new ModelProxyFactoryProvider(cfg, new ProxyGenerator());
            var paths = new List<string>();

            var modelProxy = provider.GetModelProxy(model, (path, name, obj) =>
            {
                paths.Add($"{path} {name} = {obj}");
            });

            modelProxy.Name = "not too cool";
            modelProxy.Me.PostalAddress.Postcode = "2227";
            modelProxy.Me.LastName = "Putin";
            modelProxy.PrevAddress[0].Postcode = "121467";
            var prev1 = modelProxy.PrevAddress[0];
            modelProxy.Me.ListAddresses[0].Postcode = "360";
            modelProxy.Me.ListAddresses[0].Line1 = "Mahorka";

            Assert.Equal("$.Name Name = not too cool", paths[0]);
            Assert.Equal("$.Me.PostalAddress.Postcode Postcode = 2227", paths[1]);
            Assert.Equal("$.Me.LastName LastName = Putin", paths[2]);
            Assert.Equal("$.PrevAddress[0].Postcode Postcode = 121467", paths[3]);
            Assert.Equal("$.Me.ListAddresses[0].Postcode Postcode = 360", paths[4]);
            Assert.Equal("$.Me.ListAddresses[0].Line1 Line1 = Mahorka", paths[5]);
        }

        [Fact]
        public void ProxyProviderRevertTestPull()
        {
            ProxyProviderRevertTest(new ProxyConfig(PullProxymaProvider.ProxyEngineType));
        }

        [Fact]
        public void ProxyProviderRevertTestPush()
        {
            ProxyProviderRevertTest(new ProxyConfig(PushProxymaProvider.ProxyEngineType));
        }

        internal void ProxyProviderRevertTest(ProxyConfig cfg)
        {
            var model = GetModel();
            var provider = new ModelProxyFactoryProvider(cfg, new ProxyGenerator());
            var paths = new List<string>();

            var modelProxy = provider.GetModelProxy(model, (path, name, obj) =>
            {
                paths.Add($"{path} {name} = {obj}");
            });

            modelProxy.Name = "not too cool";
            modelProxy.Me.PostalAddress.Postcode = "2227";
            modelProxy.Me.LastName = "Putin";
            modelProxy.PrevAddress[0].Postcode = "121467";
            var prev1 = modelProxy.PrevAddress[0];
            modelProxy.Me.ListAddresses[0].Postcode = "360";
            modelProxy.Me.ListAddresses[0].Line1 = "Mahorka";

            var revertedModel = provider.GetProxyModel(modelProxy);
            var json = JsonConvert.SerializeObject(revertedModel);
        }

        [Fact]
        public void ProxyProviderListInListTestPull()
        {
            ProxyProviderListInListTest(new ProxyConfig(PullProxymaProvider.ProxyEngineType));
        }

        [Fact]
        public void ProxyProviderListInListTestPush()
        {
            ProxyProviderListInListTest(new ProxyConfig(PushProxymaProvider.ProxyEngineType));
        }

        internal void ProxyProviderListInListTest(ProxyConfig cfg)
        {
            var model = GetModel();
            model.Me.ListAddresses[0].Tags = new List<string>(new string[] { "new", "cool" });
            model.Me.ListAddresses[0].Extensions.Add(new Ext { Name = "ext1" });
            model.Me.ListAddresses[0].Extensions.Add(new Ext { Name = "ext2" });
            var provider = new ModelProxyFactoryProvider(cfg, new ProxyGenerator());
            var paths = new List<string>();

            var modelProxy = provider.GetModelProxy(model, (path, name, obj) =>
            {
                paths.Add($"{path} {name} = {obj}");
            });

            modelProxy.Me.ListAddresses[0].Extensions[1].Name = "gaga";
            Assert.Equal("$.Me.ListAddresses[0].Extensions[1].Name Name = gaga", paths[0]);
        }



        //[Fact]
        public void ProxyProviderStringArrayInListTestPull()
        {
            ProxyProviderStringArrayInListTest(new ProxyConfig(PullProxymaProvider.ProxyEngineType));
        }

        internal void ProxyProviderStringArrayInListTest(ProxyConfig cfg)
        {
            var model = GetModel();
            model.Me.ListAddresses[0].Tags = new List<string>(new string[] { "new", "cool" });
            model.Me.ListAddresses[0].Extensions.Add(new Ext { Name = "ext1" });
            model.Me.ListAddresses[0].Extensions.Add(new Ext { Name = "ext2" });
            var provider = new ModelProxyFactoryProvider(cfg, new ProxyGenerator());
            var paths = new List<string>();

            var modelProxy = provider.GetModelProxy(model, (path, name, obj) =>
            {
                paths.Add($"{path} {name} = {obj}");
            });

            modelProxy.Me.ListAddresses[0].Tags[1] = "not cool bro";
            Assert.Equal("$.Me.ListAddresses[0].Tags[1] Tags[1] = not cool bro", paths[1]);
        }

        public static Model GetModel()
        {
            var model = new Model
            {
                Me = new Person
                {
                    FirstName = "Ivan",
                    PostalAddress = new Address
                    {
                        Line1 = "Sunset Ave"
                    },
                    ListAddresses = new List<Address>()
                },
                MyAddress = new Address
                {
                    Line1 = "Moscow line"
                },
                PrevAddress = new Address[] { new Address { Line1 = "Searl Road", Postcode = "2230" } },
                Name = "Super",
                //Ext = new Dictionary<string, ExpandoObject>(),
                //Expando = new ExpandoObject()
            };

            model.Me.ListAddresses.Add(new Address { Line1 = "Kiev" });

            return model;
        }

        public class ProxyConfig : IProxyScopeConfiguration
        {
            string _proxyType;
            public ProxyConfig(string proxyType)
            {
                _proxyType = proxyType;
            }
            public string GetProxyEngineType()
            {
                return _proxyType;
            }

            public IEnumerable<Type> GetScopeTypes()
            {
                return new Type[] { typeof(Model), typeof(Person), typeof(Address), typeof(Ext), typeof(DynamicRecordset), typeof(ExpandoObject) };
            }
        }

        public class Model
        {
            public virtual string Name { get; set; }

            public virtual Person Me { get; set; }
            public virtual Address MyAddress { get; set; }

            public virtual Address[] PrevAddress { get; set; }

            //public virtual Dictionary<string, ExpandoObject> Ext { get; set; }

            //public virtual ExpandoObject Expando { get; set; }
        }

        public class Person
        {
            public virtual string FirstName { get; set; }
            public virtual string LastName { get; set; }

            public virtual Address PostalAddress { get; set; }

            public virtual List<Address> ListAddresses { get; set; }
        }

        public class Address
        {
            public virtual string Line1 { get; set; }
            public virtual string Postcode { get; set; }
            public virtual List<string> Tags { get; set; }
            public virtual List<Ext> Extensions { get; set; } = new List<Ext>();
        }

        public class Ext
        {
            public virtual string Name { get; set; }
        }
    }
}
