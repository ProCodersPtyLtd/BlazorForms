using BlazorForms.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform
{
    public class CustomFormConfig
    {
        public string FullName { get; set; }
        public List<CustomFormField> Fields { get; set; } = new List<CustomFormField>();
        public List<CustomConfigDependency> Dependencies { get; set; } = new List<CustomConfigDependency>();
    }


    public class CustomFormField
    {
        public FieldControlDetails Field { get; set; }
        public bool IsOverride { get; set; }
    }

    public class CustomConfigDependency
    { 
        public string Name { get; set; }
        public string TypeCode { get; set; }
        public string ItemStoreCode { get; set; }
    }

    public static class CustomConfigDependencyCodes
    {
        public const string ModelExtension = "ModelExtension";
    }
}
