using BlazorForms.Shared;
using BlazorForms.Platform.CodeGeneration;
using BlazorForms.Platform.Crm.Artel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace BlazorForms.Platform.Tests.CodeGeneration
{
    public class FormGeneratorTests
    {
        [Fact]
        public void FindRefListShortNameTest()
        {
            var props = TypeHelper.GetNestedPublicVirtualProperties(typeof(ArtelProjectSettingsModel));
            var prop = props.FirstOrDefault(x => x.Property.Name == "BaseCurrencySearch");
            var list = FormGenerator.FindRefList(prop, props);

            Assert.NotNull(list.List);
            Assert.Equal("Name", list.Value);
            Assert.Equal("ShortName", list.Id);
        }

        [Fact]
        public void FindRefListCodeTest()
        {
            var props = TypeHelper.GetNestedPublicVirtualProperties(typeof(ArtelProjectSettingsModel));
            var prop = props.FirstOrDefault(x => x.Property.Name == "PaymentFrequencyCode");
            var list = FormGenerator.FindRefList(prop, props);

            Assert.NotNull(list.List);
            Assert.Equal("Name", list.Value);
            Assert.Equal("Code", list.Id);
        }
    }
}
