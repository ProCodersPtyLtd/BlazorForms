using BlazorForms.Shared.FastReflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Shared.Tests
{
    public class ModelBindingNavigatorTests
    {
        private readonly IModelBindingNavigator _navigator = new ModelBindingNavigator(new JsonPathNavigator());
        private readonly IFastReflectionProvider _provider = new FastReflectionProvider();

        public ModelBindingNavigatorTests()
        { }

        [Fact]
        public void GetNestedStringTest()
        {
            var m = new TestModel1 { Name = "Alla", Fomo = new Fomo { Name = "Bella" } };
            var b = new FieldBinding { BindingType = FieldBindingType.SingleField, Binding = "$.Fomo.Name" };
            _provider.UpdateBindingFastReflection(b, m.GetType());
            
            var v = _navigator.GetValue(m, b);
            Assert.Equal("Bella", v);
        }

        [Fact]
        public void GetNestedNullStringTest()
        {
            var m = new TestModel1 { Name = "Alla", Fomo = null };
            var b = new FieldBinding { BindingType = FieldBindingType.SingleField, Binding = "$.Fomo.Name" };
            _provider.UpdateBindingFastReflection(b, m.GetType());

            var v = _navigator.GetValue(m, b);
            Assert.Null(v);
        }
    }

    public class TestModel1
    {
        public string? Name { get; set; }
        public Fomo? Fomo { get; set; }
    }

    public class Fomo
    {
        public string? Name { get; set; }
    }
}
