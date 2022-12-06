using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.ItemStore
{
    public class StoreObject
    {
        public string TableName { get; set; }
        public ModelObject Model { get; set; }
        // ToDo: change to use DynamicRecordset
        public Dictionary<string, object> Values { get; set; }
        //public DynamicRecordset Values { get; set; }
    }
}
