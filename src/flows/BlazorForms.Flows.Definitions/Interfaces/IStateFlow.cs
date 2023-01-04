﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Definitions
{
	public interface IStateFlow : IFlow
	{
		List<StateDef> States { get; }
		List<TransitionDef> Transitions { get; }
		void Define();
		void Parse();
		void SetFlowContext(IFlowContext context);
		string AssignedUser { get; set; }
		string AssignedRole { get; set; }
	}
}