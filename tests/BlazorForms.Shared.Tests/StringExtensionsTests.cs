using System;
using System.Collections.Generic;
using System.Text;
using BlazorForms.Shared;
using Xunit;

namespace BlazorForms.Shared.Tests
{
    public class StringExtensionsTests
    {
        [Fact]
        public void ReplaceEndTest()
        {
            Assert.Equal("BlazorForms.Platform.Crm.Business.Artel", "BlazorForms.Platform.Crm.Business.Artel.Model".ReplaceEnd(".Model", ""));
            Assert.Equal("BlazorForms.Platform.Model.Business.Artel", "BlazorForms.Platform.Model.Business.Artel.Model".ReplaceEnd(".Model", ""));
        }

        [Fact]
        public void SwplitWords()
        {
            Assert.Equal("Project Settings", "ProjectSettings".SplitWords());
            Assert.Equal("Artel Project Dashboard", "ArtelProjectDashboard".SplitWords());
        }

        [Fact]
        public void ShortGuidTest()
        {
            ShortGuid id = ShortGuid.NewGuid();
            Assert.Equal(22, id.Value.Length);
        }

        //[Fact]
        //public void GetDescriptionTest()
        //{
        //    var flow = FlowEntityTypes.Flow.GetDescription();
        //    Assert.Equal("Flow", flow);
        //}
    }
}
