using BlazorForms.FlowRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace BlazorForms.Platform.Tests.Rules
{
    public class RuleDefinitionParserTests
    {
        [Fact]
        public void FormRuleDefinitionParseTest()
        {
            var parser = new RuleDefinitionParser(null);
            var details = parser.Parse(typeof(ProcessTask4));
            Assert.Equal(RuleTypes.ValidationRule, details.Fields[0].Rules[0].RuleType);
        }
    }
}
