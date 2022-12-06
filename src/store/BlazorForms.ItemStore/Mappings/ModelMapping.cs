using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.ItemStore
{
    public class ModelMapping
    {
        public string LeftModelName { get; set; }
        public string RightModelName { get; set; }
        public Dictionary<string, string> FieldMappings { get; set; }
    }
}
