using System;
using System.Collections.Generic;
using System.Text;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using Xunit;

namespace BlazorForms.Shared.Tests
{
    public class TypeHelperTests
    {
        [Fact]
        public void PrimitiveTypesTest()
        {
            Assert.True(typeof(DateTime).IsSimple());
            Assert.True(typeof(DateTime?).IsSimple());
            Assert.True(typeof(decimal?).IsSimple());
            Assert.True(typeof(string).IsSimple());
        }

        [Fact]
        public void PrimitiveTypesNegativeTest()
        {
            Assert.False(typeof(Model1).IsSimple());
            Assert.False(typeof(Class1).IsSimple());
        }

        [Fact]
        public void GetNestedPropertiesTest()
        {
            var mp = TypeHelper.GetNestedPublicVirtualProperties(typeof(Model3));
            Assert.Equal(6, mp.Count);
            Assert.Equal("Id3", mp[0].Property.Name);
            Assert.Null(mp[0].Parent);
            Assert.Equal("Name", mp[5].Property.Name);
            Assert.Equal("Model1Details", mp[5].Parent.Property.Name);
            Assert.Equal(typeof(Model1), mp[5].Parent.Property.PropertyType);
        }

        [Fact]
        public void GetNestedPropertiesPathTest()
        {
            var mp = TypeHelper.GetNestedPublicVirtualProperties(typeof(Model3));
            Assert.Equal(6, mp.Count);
            Assert.Equal("Model2Details.Model1Details.Name", mp[5].GetPath());
        }

        [Fact]
        public void GetNestedListPropertiesTest()
        {
            var mp = TypeHelper.GetNestedPublicVirtualProperties(typeof(ModelList));
            Assert.True(mp[0].IsList);
            Assert.False(mp[1].IsList);
        }

        [Fact]
        public void GetNestedListPropertyPathsTest()
        {
            var mp = TypeHelper.GetNestedPublicVirtualProperties(typeof(ModelList));
            Assert.Equal("Model2List", mp[0].GetPath());
            Assert.Equal("Name2", mp[1].GetPath());
            Assert.Equal("Model1Details.Name", mp[4].GetPath());
        }

        public class Class1
        {
        }

        public class Model1
        {
            public virtual string Name { get; set; }
        }

        public class Model2
        {
            public virtual string Name2 { get; set; }
            public virtual decimal? Value2 { get; set; }
            public virtual Model1 Model1Details { get; set; }
        }

        public class Model3
        {
            public virtual int Id3 { get; set; }
            public virtual Model2 Model2Details { get; set; }
        }

        public class ModelList
        {
            public virtual List<Model2> Model2List { get; set; }
        }
    }
}
