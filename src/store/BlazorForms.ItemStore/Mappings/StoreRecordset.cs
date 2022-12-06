using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.ItemStore
{
    public class StoreRecordset
    {
        public ModelObject Model { get; set; }
        //public Dictionary<string, StoreField> Fields;
        // <FieldName, Values[]>
        public Dictionary<string, object[]> Records { get; set; }
    }

    //public class StoreField
    //{
    //    public string Name { get; set; }
    //    public string Type { get; set; }
    //}
}
