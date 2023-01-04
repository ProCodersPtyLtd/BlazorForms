using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Definitions
{
    public class StateFlowObject
    {
        public readonly string Value;
        public readonly string Caption;

        public StateFlowObject(string value, string label)
        {
            Value = value;
            Caption = label;
        }
    }
    public class state : StateFlowObject
    {
        public state(string label) : base(null, label)
        {

        }

		public state(string value, string label) : base(value, label)
		{

		}
	}
    public class status : StateFlowObject
    {
		public status(string label) : base(null, label)
		{

		}

		public status(string value, string label) : base(value, label)
        {

        }
    }

}
