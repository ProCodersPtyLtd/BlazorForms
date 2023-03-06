using System;
using System.Collections.Generic;
using System.Text;
using BlazorForms.Shared.Tests.Models;
using BlazorForms.Shared;
using BlazorForms.Shared.DataStructures;
using Xunit;

namespace BlazorForms.Shared.Tests
{
    public class JsonPathNavigatorTests
    {
        [Fact]
        public void GetNestedValueTest()
        {
            var nav = new JsonPathNavigator();
            var model = new Model1 { Client = new ClientModel { FirstName = "Lilu", ResidentialAddress = new AddressModel { StreetLine1 = "14 Sunset Ave" } } };
            var result = nav.GetValue(model, "$.Client.FirstName").AsString();
            Assert.Equal("Lilu", result);
            result = nav.GetValue(model, "$.Client.LastName").AsString();
            Assert.Null(result);
            result = nav.GetValue(model, "$.Client.ResidentialAddress.StreetLine1").AsString();
            Assert.Equal("14 Sunset Ave", result);
        }

        [Fact]
        public void GetNestedValueNullTest()
        {
            var nav = new JsonPathNavigator();
            var model = new Model1 { Client = new ClientModel { FirstName = "Lilu", ResidentialAddress = null } };
            var result = nav.GetValue(model, "$.Client.FirstName").AsString();
            Assert.Equal("Lilu", result);
            result = nav.GetValue(model, "$.Client.LastName").AsString();
            Assert.Null(result);
            result = nav.GetValue(model, "$.Client.ResidentialAddress.StreetLine1").AsString();
            Assert.Null(result);
        }

        [Fact]
        public void GetWrongMappingValueTest()
        {
            var nav = new JsonPathNavigator();
            var model = new Model1 { Client = new ClientModel { FirstName = "Lilu", ResidentialAddress = new AddressModel { StreetLine1 = "14 Sunset Ave" } } };
            var result = nav.GetValue(model, "$.Client.Hahaha").AsString();
            Assert.Null(result);
        }

        [Fact]
        public void GetMoneyValueTest()
        {
            var nav = new JsonPathNavigator();
            var model = new Model1 { Total = new Money()};
            model.Total.Amount = 237m;
            model.Total.Currency = "ETH";
            Assert.Equal(237m, nav.GetValue(model, "$.Total.Amount").AsDecimal());
            Assert.Equal("ETH", nav.GetValue(model, "$.Total.Currency").AsString());
        }

        [Fact]
        public void GetMoneyValueNullTest()
        {
            var nav = new JsonPathNavigator();
            var model = new Model1 { Total = new Money() };
            model.Total.Amount = null;
            model.Total.Currency = null;
            Assert.Null(nav.GetValue(model, "$.Total.Amount").AsDecimal());
            Assert.Null(nav.GetValue(model, "$.Total.Currency").AsString());
        }
    }

    public class Model1
    {
        public virtual ClientModel Client { get; set; }
        public virtual Money Total { get; set; }
        public virtual string SomeName { get; set; }
        public virtual int SomeInt { get; set; }
        public virtual int? NullableInt { get; set; }
        public virtual decimal SomeDecimal { get; set; }
    }
}
