using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        // FieldSetGroup
        public List<FieldControlDetails> FieldSet { get; set; }
        public string GroupBinding { get; set; }
        public FieldBindingType BindingType { get; set; }

        public FieldControlDetails()
        { }

        public FieldControlDetails(string groupBinding, IEnumerable<FieldControlDetails> fields)
        {
            GroupBinding = groupBinding;
            FieldSet = fields.ToList();
            BindingType = fields.First().Binding.BindingType;
        }

        public FieldControlDetails FindField(ControlType controlType)
        {
            var result = FieldSet.FirstOrDefault(x => x.ControlType == controlType.ToString());
            return result;
        }

        public static List<FieldControlDetails> FindAllSets(IEnumerable<FieldControlDetails> fields)
        {
            var result = fields.GroupBy(x => x.FieldSetGroup).Select(g => new FieldControlDetails(g.Key, g)).ToList();
            return result;
        }
    }
}
