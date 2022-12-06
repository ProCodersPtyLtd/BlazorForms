using BlazorForms.Shared;
using BlazorForms.Shared.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BlazorForms.Shared.Tests
{
    public class JsonPathHelperTests
    {
        [Fact]
        public void RemoveLastPropertyTest()
        {
            Assert.Equal("$.Price", JsonPathHelper.RemoveLastProperty("$.Price.Amount"));
            Assert.Equal("$.", JsonPathHelper.RemoveLastProperty("$.Price"));
            Assert.Equal("$.", JsonPathHelper.RemoveLastProperty("$."));
            Assert.Equal("", JsonPathHelper.RemoveLastProperty("$"));
            Assert.Equal("", JsonPathHelper.RemoveLastProperty("$IncorrectPath"));
            Assert.Equal("", JsonPathHelper.RemoveLastProperty(""));
        }

        [Fact]
        public void GetTypeIterateThroughPathTest()
        {
            var subType = JsonPathHelper.GetTypeIterateThroughPath(typeof(ClientModel), "$.PostAddress");
            Assert.Equal(typeof(AddressModel), subType);
        }

        [Fact]
        public void GetTypeIterateThroughPathItemsTest()
        {
            var subType = JsonPathHelper.GetItemsType(typeof(ClientModel), "$.AddressList"); 
            Assert.Equal(typeof(AddressModel), subType);
        }
    }
}
