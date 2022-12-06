using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class FlowAttribute : Attribute
    {
        public string Name { get; set; }
        public FlowAttribute(string name) : base()
        {
            Name = name;
        }

        public Type ChildFlow { get; set; }
        public Type DefaultReadonlyView { get; set; }
    }

    public class TaskAttribute : Attribute
    {
    }

    public class SaveTaskAttribute : Attribute
    {
    }

    public class LoadTaskAttribute : Attribute
    {
    }

    

    //public enum AccessTypes
    //{
    //    Allow,
    //}
}
