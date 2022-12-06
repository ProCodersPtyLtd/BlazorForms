using System;
using System.Collections.Generic;
using System.Text;
using BlazorForms.Shared.Tests.Models;
using BlazorForms.Shared;
using BlazorForms.Shared.DataStructures;
using Xunit;
using BlazorForms.Shared.Reflection;

namespace BlazorForms.Shared.Tests
{
    public class JsonPathNavigatorFastEmittedTests
    {
        [Fact]
        public void PropertyInforHashTest()
        {
            var m1 = new Model1 { SomeInt = 301 };
            var m2 = new Model1 { SomeInt = 302 };
            var m3 = new Model2 { SomeInt = 302 };
            var p1 = m1.GetType().GetProperty("SomeInt");
            var p2 = m2.GetType().GetProperty("SomeInt");
            var p3 = m3.GetType().GetProperty("SomeInt");

            Assert.Equal(p1.GetHashCode(), p2.GetHashCode());
            Assert.NotEqual(p1.GetHashCode(), p3.GetHashCode());
        }

        [Fact]
        public void GetValueNestedDictionaryIntTest()
        {
            var nav = new FastReflectionPrototype();
            var model = new Model1 { SomeInt = 301 };
            var result = nav.GetValueNestedDictionary(model, "SomeInt").AsInt();
            Assert.Equal(301, result);

            result = nav.GetValueNestedDictionary(model, "SomeInt").AsInt();
            Assert.Equal(301, result);
        }

        [Fact]
        public void GetValueNestedDictionaryDecimalTest()
        {
            var nav = new FastReflectionPrototype();
            var model = new Model1 { SomeDecimal = 301_000.00m };
            var result = nav.GetValueNestedDictionary(model, "SomeDecimal").AsDecimal();
            Assert.Equal(301_000.00m, result);
        }

        [Fact]
        public void GetValueNestedDictionaryNullableIntTest()
        {
            var nav = new FastReflectionPrototype();
            var model = new Model1 { NullableInt = 301 };
            var result = nav.GetValueNestedDictionary(model, "NullableInt").AsInt();
            Assert.Equal(301, result);

            result = nav.GetValueNestedDictionary(model, "NullableInt").AsInt();
            Assert.Equal(301, result);
        }

        [Fact]
        public void SetValueNestedDictionaryTest()
        {
            var nav = new FastReflectionPrototype();
            var model = new Model1 { SomeName = "Sunset" };
            nav.SetValueNestedDictionary(model, "SomeName", "bla");
            Assert.Equal("bla", model.SomeName);
            nav.SetValueNestedDictionary(model, "SomeName", "bla2");
            Assert.Equal("bla2", model.SomeName);
        }

        [Fact]
        public void SetValueNestedDictionaryIntTest()
        {
            var nav = new FastReflectionPrototype();
            var model = new Model1();
            nav.SetValueNestedDictionary(model, "SomeInt", 23);
            Assert.Equal(23, model.SomeInt);
            nav.SetValueNestedDictionary(model, "SomeInt", 233);
            Assert.Equal(233, model.SomeInt);
        }

        [Fact]
        public void SetValueNestedDictionaryNullableIntTest()
        {
            var nav = new FastReflectionPrototype();
            var model = new Model1 { NullableInt = 3 };
            nav.SetValueNestedDictionary(model, "NullableInt", 5);
            Assert.Equal(5, model.NullableInt);
        }

        [Fact]
        public void SetValueNestedDictionaryNullableIntNullTest()
        {
            var nav = new FastReflectionPrototype();
            var model = new Model1 { NullableInt = 3 };
            nav.SetValueNestedDictionary(model, "NullableInt", null);
            Assert.Null(model.NullableInt);
        }

        [Fact]
        public void GetSetPropertyEmitterTest()
        {
            var nav = new FastReflectionPrototype();
            var model = new Model1 { SomeName = "Sunset" };
            var propertyInfo = model.GetType().GetProperty("SomeName");
            var method = FastReflectionPrototype.GetSetPropertyEmitter(model.GetType(), "SomeName", propertyInfo).CreateDelegate();
            method(model, "bla");

            Assert.Equal("bla", model.SomeName);
        }

        [Fact]
        public void GetValueTupleKeyRepeatTest()
        {
            var nav = new FastReflectionPrototype();
            var model = new Model1 { SomeName = "Sunset" };
            var result = nav.GetValueTupleKey(model, "SomeName").AsString();
            Assert.Equal("Sunset", result);

            result = nav.GetValueTupleKey(model, "SomeName").AsString();
            Assert.Equal("Sunset", result);

            result = nav.GetValueTupleKey(model, "SomeName").AsString();
            Assert.Equal("Sunset", result);
        }

        [Fact]
        public void GetValueNestedDictionaryRepeatTest()
        {
            var nav = new FastReflectionPrototype();
            var model = new Model1 { SomeName = "Sunset" };
            var result = nav.GetValueNestedDictionary(model, "SomeName").AsString();
            Assert.Equal("Sunset", result);

            result = nav.GetValueNestedDictionary(model, "SomeName").AsString();
            Assert.Equal("Sunset", result);

            result = nav.GetValueNestedDictionary(model, "SomeName").AsString();
            Assert.Equal("Sunset", result);
        }

        [Fact]
        public void GetValueRepeatTest()
        {
            var nav = new JsonPathNavigatorFastEmitted();
            var model = new Model1 { SomeName = "Sunset" };
            var result = nav.GetValue(model, "$.SomeName").AsString();
            Assert.Equal("Sunset", result);

            result = nav.GetValue(model, "$.SomeName").AsString();
            Assert.Equal("Sunset", result);

            result = nav.GetValue(model, "$.SomeName").AsString();
            Assert.Equal("Sunset", result);
        }

        [Fact]
        public void GetNestedValueTest()
        {
            var nav = new JsonPathNavigatorFastEmitted();
            var model = new Model1 { Client = new ClientModel { FirstName = "Lilu", ResidentialAddress = new AddressModel { StreetLine1 = "14 Sunset Ave" } } };
            var result = nav.GetValue(model, "$.Client.FirstName").AsString();
            Assert.Equal("Lilu", result);
            result = nav.GetValue(model, "$.Client.LastName").AsString();
            Assert.Null(result);
            result = nav.GetValue(model, "$.Client.ResidentialAddress.StreetLine1").AsString();
            Assert.Equal("14 Sunset Ave", result);
        }

        [Fact]
        public void GetWrongMappingValueTest()
        {
            var nav = new JsonPathNavigatorFastEmitted();
            var model = new Model1 { Client = new ClientModel { FirstName = "Lilu", ResidentialAddress = new AddressModel { StreetLine1 = "14 Sunset Ave" } } };
            var result = nav.GetValue(model, "$.Client.Hahaha").AsString();
            Assert.Null(result);
        }

        [Fact]
        public void GetMoneyValueTest()
        {
            var nav = new JsonPathNavigatorFastEmitted();
            var model = new Model1 { Total = new Money()};
            model.Total.Amount = 237m;
            model.Total.Currency = "ETH";
            Assert.Equal("ETH", nav.GetValue(model, "$.Total.Currency").AsString());
            Assert.Equal(237m, nav.GetValue(model, "$.Total.Amount").AsDecimal());
        }

        [Fact]
        public void GetMoneyValueNullTest()
        {
            var nav = new JsonPathNavigatorFastEmitted();
            var model = new Model1 { Total = new Money() };
            model.Total.Amount = null;
            model.Total.Currency = null;
            Assert.Null(nav.GetValue(model, "$.Total.Amount").AsDecimal());
            Assert.Null(nav.GetValue(model, "$.Total.Currency").AsString());
        }

        public class Model2
        {
            public virtual ClientModel Client { get; set; }
            public virtual Money Total { get; set; }
            public virtual string SomeName { get; set; }
            public virtual int SomeInt { get; set; }
            public virtual int? NullableInt { get; set; }
            public virtual decimal SomeDecimal { get; set; }
        }
    }
}
