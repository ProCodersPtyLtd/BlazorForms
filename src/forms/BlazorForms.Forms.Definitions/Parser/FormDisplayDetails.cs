using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Forms
{
    public class FormDisplayDetails
    {
        public string Name { get; set; }
        public string Caption { get; set; }
        public bool Visible { get; set; }
        public bool Disabled { get; set; }
        public bool Required { get; set; }
        public bool Highlighted { get; set; }
        public bool Password { get; set; }
        public string Hint { get; set; }
        public bool NoCaption { get; set; }
        //public string ModelBinding { get; set; }
        public bool? IsPrimaryKey { get; set; }
        public bool IsUnique { get; set; }
        public FieldFilterType FilterType { get; set; }
        public string FilterRefField { get; set; }
        public string Format { get; set; }

        // ToDo: can it be readonly copy of the field binding?
        public FieldBinding Binding { get; set; }
    }
}
