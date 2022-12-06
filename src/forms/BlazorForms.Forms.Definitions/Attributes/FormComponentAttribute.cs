using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Forms
{
    public class FormComponentAttribute : Attribute
    {
        public string Caption { get; set; }
        public string Group { get; set; }
        public Type FormComponentType { get; set; }

        

        public FormComponentAttribute(Type formComponentType, string caption = null)
        {
            FormComponentType = formComponentType;
            Caption = caption;
        }
    }
}
