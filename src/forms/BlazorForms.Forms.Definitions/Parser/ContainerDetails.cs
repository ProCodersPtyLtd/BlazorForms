using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BlazorForms.Forms
{
    public class ContainerDetails
    {
        public string Name { get; set; }
        public FormLayout Layout { get; set; }
        public string Group { get; set; }
        public string Caption { get; set; }
        public string ControlType { get; set; }
        public FormDisplayDetails DisplayProperties { get; set; }
        public List<FieldControlDetails> Fields { get; set; }
        public Collection<FormFlowRuleDetails> FlowRules { get; set; } = new();
        public int Order { get; set; }
        public string ItemDialogFlow { get; set; }
        public string FieldSetGroup { get; set; }

        public bool IsListNotRenderedField 
        { 
            get 
            { 
                return ControlType == Forms.ControlType.Table.ToString() 
                    || ControlType == Forms.ControlType.ActionMenuItem.ToString()
                    || ControlType == Forms.ControlType.Form.ToString();
            } 
        }

        public bool IsNotRenderedField
        {
            get
            {
                return ControlType == Forms.ControlType.Form.ToString();
            }
        }

        public bool IsRenderedField { get { return !IsNotRenderedField; } }
        public bool IsListRenderedField { get { return !IsListNotRenderedField; } }
    }
}
