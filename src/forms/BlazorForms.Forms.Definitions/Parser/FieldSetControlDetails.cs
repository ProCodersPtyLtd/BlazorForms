//using BlazorForms.Shared;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BlazorForms.Forms
//{
//    public class FieldSetControlDetails
//    {
//        public string GroupBinding { get; set; }
//        public FieldBindingType BindingType { get; set; }
//        public List<FieldControlDetails> Fields { get; set; } = new();

//        public FieldSetControlDetails() { }

//        public FieldSetControlDetails(string groupBinding, IEnumerable<FieldControlDetails> fields)
//        {
//            GroupBinding = groupBinding;
//            Fields = fields.ToList();
//            BindingType = fields.First().Binding.BindingType;
//        }

//        public FieldControlDetails FindField(ControlType controlType)
//        {
//            var result = Fields.FirstOrDefault(x => x.ControlType == controlType.ToString());
//            return result;
//        }

//        public static List<FieldSetControlDetails> FindAllSets(IEnumerable<FieldControlDetails> fields)
//        {
//            var result = fields.GroupBy(x => x.FieldSetGroup).Select(g => new FieldSetControlDetails(g.Key, g)).ToList();
//            return result;
//        }
//    }
//}
