using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Forms
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FormAttribute : Attribute
    {
        public Type ChildProcess { get; set; }
        public string Name { get; set; }

        public FormAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// When applied to form base class informs the framework to look for field defenitions in the base class as well
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BaseFormAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class AllowAccessAttribute : Attribute
    {
        public bool Allow { get; set; }
        public bool OnlyAssignee { get; set; }
        public string Roles { get; set; }
        public Type CustomRule { get; set; }

        public AllowAccessAttribute(bool allow)
        {
            Allow = allow;
        }
    }
}
