﻿using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine.StateFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows
{
    public class FlowDefinitionDetails
    {
        public List<StateDef> States { get; internal set; } = new();
        public List<TransitionDef> Transitions { get; internal set; } = new();
        public List<FormDef> Forms { get; internal set; }
        public string CurrentState { get; internal set; }
        public List<TransitionDef> CurrentStateTransitions { get; internal set; }
        public Dictionary<TransitionDef, StateFlowTransitionSelector> CurrentStateSelectors { get; internal set; } = new Dictionary<TransitionDef, StateFlowTransitionSelector>();
    }
}
