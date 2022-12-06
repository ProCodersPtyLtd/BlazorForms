using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace BlazorForms.Shared
{
    public class DynamicRecordset
    {
        public DynamicRecordset()
        {

        }

        public DynamicRecordset(ExpandoObject src)
        {
            Data = src;
        }

        public DynamicRecordset(Dictionary<string, object> src)
        {
            Data = new ExpandoObject();
            Data.CopyFrom(src);
        }

        public DynamicRecordset(List<ExpandoObject> src)
        {
            Rows = new List<ExpandoObject>(src);
        }

        public virtual ModelObject Model { get; set; }
        public virtual List<ExpandoObject> Rows { get; set; }
        public virtual ExpandoObject Data { get; set; } 

        public bool SingleValue 
        { 
            get 
            { 
                return Data != null; 
            } 
        }
        public bool Empty 
        { 
            get 
            { 
                return Data == null || Rows == null || (Rows != null && Rows.Count == 0); 
            } 
        }
    }

    public class ModelObject
    {
        public string Name { get; set; }
        public Dictionary<string, ModelField> Fields { get; set; } = new Dictionary<string, ModelField>();
    }

    public class ModelField
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
