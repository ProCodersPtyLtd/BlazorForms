using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Forms
{
    public class FieldRule
    {
        public Type RuleType { get; set; }
        public FormRuleTriggers Trigger { get; set; }
        public Type EntityType { get; set; }
        public bool IsOuterProperty { get; set; }
    }
}
