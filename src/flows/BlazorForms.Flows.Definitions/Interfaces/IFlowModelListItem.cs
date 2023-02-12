using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Definitions
{
	public interface IFlowModelListItem
	{
		public bool Changed { get; set; }
		public bool Deleted { get; set; }
	}
}
