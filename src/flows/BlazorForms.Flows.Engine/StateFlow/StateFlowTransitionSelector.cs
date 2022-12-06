using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Engine.StateFlow
{
    public class StateFlowTransitionSelector
    {
        public List<string> Values { get; set; }
        //public string SelectedValue { get; set; }

        public StateFlowTransitionSelector()
        { }

        public StateFlowTransitionSelector(IEnumerable<string> values)
        {
            Values = values.ToList();
        }
    }
}
