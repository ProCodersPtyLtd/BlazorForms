using BlazorForms.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BlazorForms.Platform.Tests.FluentForms
{
    public class ControlTypeTests
    {
        [Fact]
        public void ControlTypeEnumToStringTest()
        {
            var control = ControlType.Autocomplete;
            var name = control.ToString();
            Assert.Equal("Autocomplete", name);
        }
    }
}
