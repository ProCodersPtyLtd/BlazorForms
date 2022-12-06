using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows
{
    public class StateFlowObject
    {
        public readonly string Value;

        public StateFlowObject(string value)
        {
            Value = value;
        }
    }
    public class state : StateFlowObject
    {
        public state(string value) : base(value)
        {

        }
    }
    public class status : StateFlowObject
    {
        public status(string value) : base(value)
        {

        }
    }

}
