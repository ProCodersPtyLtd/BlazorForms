using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Definitions
{
	public interface IStateFlow : IFlow
	{
		Func<Task> OnBeginAsync { get; internal set; }
		List<StateDef> States { get; }
		List<TransitionDef> Transitions { get; }
		List<FormDef> Forms { get; }
		void Define();
		void Parse();
		void SetFlowContext(IFlowContext context);
		string AssignedUser { get; set; }
		string AssignedRole { get; set; }
	}
}
