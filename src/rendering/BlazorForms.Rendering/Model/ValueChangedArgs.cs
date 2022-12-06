using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.State
{
    public class ValueChangedArgs
    { 
        public FieldBinding Binding { get; set; }
        //public string ModelBinding { get; set; }
        public object NewValue { get; set; }
        public int RowIndex { get; set; }
        public ModelChangedOperation Operation { get; set; }
    }

    public enum ModelChangedOperation
    {
        Default = 0,
        Refresh,
        SubmitAndRefresh
    }   
}
