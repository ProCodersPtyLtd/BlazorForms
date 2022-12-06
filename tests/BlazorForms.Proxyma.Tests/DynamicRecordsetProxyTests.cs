using BlazorForms.Proxyma;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Castle.DynamicProxy;
using System.Dynamic;
using BlazorForms.Shared;
using System.ComponentModel;
using BlazorForms.Shared.Reflection;
using BlazorForms.Flows.Definitions;

namespace BlazorForms.Proxyma.Tests
{
    public class DynamicRecordsetProxyTests
    {
        [Fact]
        public void ExpandoSetGetTest()
        {
            var eo = new ExpandoObject();
            int changedValue = 0;
            string changedProp = "";

            ((INotifyPropertyChanged)eo).PropertyChanged += (o, p) =>
            {
                changedProp = p.PropertyName;
                changedValue = (int)eo.GetValue(changedProp);
            };

            eo.SetValue("property1", 25);

            Assert.Equal(25, changedValue);
            Assert.Equal("property1", changedProp);
        }

        [Fact]
        public void ProxyProviderExpandoTestPull()
        {
            var model = GetModel();
            model.Expando.SetValue("Id", 23);
            model.Expando.SetValue("Name", "barabulka");
            var provider = new ModelProxyFactoryProvider(new ProxyProvidersHierarchyTests.ProxyConfig(PullProxymaProvider.ProxyEngineType), new ProxyGenerator());
            var paths = new List<string>();

            var modelProxy = provider.GetModelProxy(model, (path, name, obj) =>
            {
                paths.Add($"{path} {name} = {obj}");
            });

            modelProxy.Expando.SetValue("Name", "gazon");
            Assert.Equal("$.Expando.Name Name = gazon", paths[0]);
        }

        [Fact]
        public void ProxyProviderExpandoRevertedNotSubscribedPullTest()
        {
            var model = GetModel();
            model.Expando.SetValue("Id", 23);
            model.Expando.SetValue("Name", "barabulka");
            var provider = new ModelProxyFactoryProvider(new ProxyProvidersHierarchyTests.ProxyConfig(PullProxymaProvider.ProxyEngineType), new ProxyGenerator());
            var paths = new List<string>();

            var modelProxy = provider.GetModelProxy(model, (path, name, obj) =>
            {
                paths.Add($"{path} {name} = {obj}");
            });

            modelProxy.Expando.SetValue("Name", "gazon");
            Assert.Single(paths);

            var modelReverted = provider.GetProxyModel(modelProxy);
            modelReverted.Expando.SetValue("Name", "liaz");
            Assert.Single(paths);
        }

        [Fact]
        public void ProxyProviderExpandoInDictionaryTestPull()
        {
            var model = GetModel();
            model.Ext["t1"] = new ExpandoObject();
            //model.Ext["t1"].SetValue("project_id", 17);
            var provider = new ModelProxyFactoryProvider(new ProxyProvidersHierarchyTests.ProxyConfig(PullProxymaProvider.ProxyEngineType), new ProxyGenerator());
            var paths = new List<string>();

            var modelProxy = provider.GetModelProxy(model, (path, name, obj) =>
            {
                paths.Add($"{path} {name} = {obj}");
            });

            modelProxy.Ext["t1"].SetValue("allocation_id", 17);
            Assert.Equal("$.Ext['t1'].allocation_id allocation_id = 17", paths[0]);
        }

        [Fact]
        public void ProxyProviderExpandoInDictionaryRevertedNotSubscribedTestPull()
        {
            var model = GetModel();
            model.Ext["t1"] = new ExpandoObject();
            //model.Ext["t1"].SetValue("project_id", 17);
            var provider = new ModelProxyFactoryProvider(new ProxyProvidersHierarchyTests.ProxyConfig(PullProxymaProvider.ProxyEngineType), new ProxyGenerator());
            var paths = new List<string>();

            var modelProxy = provider.GetModelProxy(model, (path, name, obj) =>
            {
                paths.Add($"{path} {name} = {obj}");
            });

            modelProxy.Ext["t1"].SetValue("allocation_id", 17);
            Assert.Single(paths);

            var modelReverted = provider.GetProxyModel(modelProxy);
            modelReverted.Ext["t1"].SetValue("allocation_id", 19);
            Assert.Single(paths);
        }

        [Fact]
        public void ProxyProviderFlowModelTestPull()
        {
            var model = new DynamicTestModel();
            var t1 = new ExpandoObject();
            model.Ext["t1"] = new DynamicRecordset(t1);
            //model.Ext["t1"].SetValue("project_id", 17);
            var provider = new ModelProxyFactoryProvider(new ProxyProvidersHierarchyTests.ProxyConfig(PullProxymaProvider.ProxyEngineType), new ProxyGenerator());
            var paths = new List<string>();

            var modelProxy = provider.GetModelProxy(model, (path, name, obj) =>
            {
                paths.Add($"{path} {name} = {obj}");
            });

            var dict = modelProxy.Ext["t1"];
            modelProxy.Ext["t1"].Data.SetValue("allocation_id", 17);
            Assert.Single(paths);
            Assert.Equal("$.Ext['t1'].Data.allocation_id allocation_id = 17", paths[0]);

            var modelReverted = provider.GetProxyModel(modelProxy);
            modelReverted.Ext["t1"].Data.SetValue("allocation_id", 19);
            Assert.Single(paths);
        }

        [Fact]
        public void JsonPathNavigatorGetDynamicRecordsetTest()
        {
            var model = new DynamicTestModel();
            var t1 = new ExpandoObject();
            model.Ext["t1"] = new DynamicRecordset(t1);
            model.Ext["t1"].Data.SetValue("allocation_id", 17);
            var navi = new JsonPathNavigator();
            var id = (int)navi.GetValue(model, "$.Ext['t1'].Data.allocation_id");
            Assert.Equal(17, id);
        }

        [Fact]
        public void JsonPathNavigatorSetDynamicRecordsetTest()
        {
            var model = new DynamicTestModel();
            var t1 = new ExpandoObject();
            model.Ext["t1"] = new DynamicRecordset(t1);
            model.Ext["t1"].Data.SetValue("allocation_id", 0);
            var navi = new JsonPathNavigator();
            navi.SetValue(model, "$.Ext['t1'].Data.allocation_id", 23);
            var id = (int)model.Ext["t1"].Data.GetValue("allocation_id");
            Assert.Equal(23, id);
        }

        public class DynamicTestModel : FlowModelBase
        { }

        public static Model GetModel()
        {
            var model = new Model
            {
                //Me = new Person
                //{
                //    FirstName = "Ivan",
                //    PostalAddress = new Address
                //    {
                //        Line1 = "Sunset Ave"
                //    },
                //    ListAddresses = new List<Address>()
                //},
                //MyAddress = new Address
                //{
                //    Line1 = "Moscow line"
                //},
                //PrevAddress = new Address[] { new Address { Line1 = "Searl Road", Postcode = "2230" } },
                Name = "Super",
                Ext = new Dictionary<string, ExpandoObject>(),
                Expando = new ExpandoObject()
            };

            //model.Me.ListAddresses.Add(new Address { Line1 = "Kiev" });

            return model;
        }

        public class Model
        {
            public virtual string Name { get; set; }

            //public virtual Person Me { get; set; }
            //public virtual Address MyAddress { get; set; }

            //public virtual Address[] PrevAddress { get; set; }

            public virtual Dictionary<string, ExpandoObject> Ext { get; set; }

            public virtual ExpandoObject Expando { get; set; }
        }
    }
}
