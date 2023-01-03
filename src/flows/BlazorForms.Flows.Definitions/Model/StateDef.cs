using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Definitions
{
	public class StateDef
	{
		public string State { get; set; }
		public bool IsEnd { get; internal set; }
	}
}
