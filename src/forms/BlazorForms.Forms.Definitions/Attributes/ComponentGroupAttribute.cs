using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Forms
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ComponentGroupAttribute : Attribute
    {
        public string Name { get; set; }

        public ComponentGroupAttribute(string groupName)
        {
            Name = groupName;
        }
    }
}
