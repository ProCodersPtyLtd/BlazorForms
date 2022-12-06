using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform
{
    public class CustomModelConfig
    {
        public string FlowName { get; set; }
        public string TaskName { get; set; }

        public List<CustomModel> Models { get; set; } = new List<CustomModel>();
    }

    public class CustomModel
    {
        public string Name { get; set; }
        public string PkJsonPath { get; set; }
        public StoreQueryReference Query { get; set; }
        public ModelObjectMapping Mapping { get; internal set; }
    }

    public class ModelObjectMapping
    {
        public string LeftModelName { get; set; }
        public string RightModelName { get; set; }
        public Dictionary<string, string> FieldMappings { get; set; }
    }

    public class StoreQueryReference
    {
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string QueryName { get; set; }
    }
}
