using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BlazorForms.Platform.Tests.Flows
{
    public class FlowParamsTests
    {
        [Fact]
        public void ItemIntAboveZeroNegativeTest()
        {
            var p = new FlowParamsGeneric();
            p.ItemId = null;
            Assert.False(p.ItemKeyAboveZero);
        }

        [Fact]
        public void ItemIntAboveZeroTest()
        {
            var p = new FlowParamsGeneric();
            p.ItemId = "1";
            Assert.True(p.ItemKeyAboveZero);
        }
    }
}
