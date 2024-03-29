﻿using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Forms
{
    public class DisplayAttribute : Attribute
    {
        public string Caption { get; set; }
        public bool Visible { get; set; } = true;
        public bool Disabled { get; set; }
        public bool Required { get; set; }
        public bool Highlighted { get; set; }
        public bool Password { get; set; }
        public string Hint { get; set; }
        public bool NoCaption { get; set; }
        public string FilterRefField { get; set; }

        public bool IsPrimaryKey { get; set; }

        public FieldFilterType FilterType { get; set; }

        public DisplayAttribute()
        {

        }

        public DisplayAttribute(string caption)
        {
            Caption = caption;
        }
    }
}
