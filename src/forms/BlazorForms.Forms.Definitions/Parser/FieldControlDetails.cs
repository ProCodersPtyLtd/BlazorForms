using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BlazorForms.Forms
{
    public class FieldControlDetails: ContainerDetails
    {
        //public string ModelBinding { get; set; }
        //public string ModelBindingType { get; set; }
        //public string ModelItems { get; set; }
        //public string ModelItemId { get; set; }
        //public string ModelItemName { get; set; }
        //public string ModelTableBinding { get; set; }
        //public string ModelTargetBinding { get; set; }

        // new binding concept
        public FieldBinding Binding { get; set; }

        // Navigation
        public string ActionLink { get; set; }
    }
}
