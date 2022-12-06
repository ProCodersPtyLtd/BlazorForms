using System;

namespace BlazorForms.Shared
{
    public enum FlowReferenceOperation
    {
        Unknown = 0,
        Add = 1,
        Edit,
        Delete,
        Resend,
        Reset,
        Invite,
        Export,
        New,
        Custom,
        DialogForm,
        View,
        Details,
        QuickAction,
        OpenInNewTab
    }

    public interface IBindingFlowReference
    {
        //string Name { get; }
        //Type FlowType { get; }
        BindingFlowAction GetAction();
    }


    public class BindingFlowReference : IBindingFlowReference
    {
        public const string EditTag = "EDIT";

        public string Name { get; set; }
        public Type FlowType { get; set; }
        public FlowReferenceOperation Operation { get; set; }
        public string Tag { get; private set; }

        public BindingFlowReference(string name, Type flowType, FlowReferenceOperation? operation = null)
        {
            if(operation != null)
            {
                Operation = operation.Value;
            }

            Name = name;
            FlowType = flowType;
        }

        public BindingFlowReference(string name, Type flowType, string tag)
        {
            Tag = tag;
            Name = name;
            FlowType = flowType;
        }

        public BindingFlowAction GetAction()
        {
            return new BindingFlowAction { Name = Name, FlowFullName = FlowType?.FullName, Operation = Operation, Tag = Tag };
        }
    }

    public class BindingFlowNavigationReference : IBindingFlowReference
    {
        public string Name { get; private set; }
        public string NavigationFormat { get; private set; }
        public FlowReferenceOperation Operation { get; private set; }
        public string Tag { get; private set; }

        public BindingFlowNavigationReference(string name, string navigationFormat, FlowReferenceOperation? operation = null)
        {
            if (operation != null)
            {
                Operation = operation.Value;
            }

            Name = name;
            NavigationFormat = navigationFormat;
        }

        public BindingFlowNavigationReference(string name, string navigationFormat, string tag)
        {
            Tag = tag;
            Name = name;
            NavigationFormat = navigationFormat;
        }

        public BindingFlowAction GetAction()
        {
            return new BindingFlowAction { Name = Name, Operation = Operation, Tag = Tag, IsNavigation = true, NavigationFormat = NavigationFormat };
        }
    }
}
