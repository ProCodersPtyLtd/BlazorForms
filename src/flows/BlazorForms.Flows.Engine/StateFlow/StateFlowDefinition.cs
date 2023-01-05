using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Flows.Definitions;

namespace BlazorForms.Flows
{
    public static class StateFlowDefinition
    {
        public static F State<F>(this F flow, state State) where F : class, IStateFlow { RegisterState(flow, State); return flow; }

		public static F EditForm<F, TForm>(this F flow) 
            where F : class, IStateFlow 
            where TForm : class
		{
            RegisterForm(flow, typeof(TForm));
			return flow; 
        }

		public static StateFlowBase EditForm<TForm>(this StateFlowBase flow)
			where TForm : class
		{
			RegisterForm(flow, typeof(TForm));
			return flow;
		}


		public static F Transition<F>(this F flow, Func<TransitionTrigger> triggerFunction, state state, Action onTransitionEvent = null) 
            where F : class, IStateFlow 
        { 
            RegisterTransition(flow, triggerFunction, state, onTransitionEvent); 
            return flow; 
        }

        public static F Transition<F>(this F flow, TransitionTrigger trigger, state state, Action onTransitionEvent = null) where F : class, IStateFlow 
        { 
            if (trigger.Text == null)
            {
                trigger.Text = state.Value;
			}

            RegisterTransition(flow, trigger, state, onTransitionEvent); 
            return flow; 
        }

		public static F Transition<F>(this F flow, UserActionTransitionTrigger trigger, state state, Action onTransitionEvent = null) where F : class, IStateFlow
		{
			if (trigger.Text == null)
			{
				trigger.Text = state.Caption;
				trigger.CommandText = state.Value;
			}

			RegisterTransition(flow, trigger, state, onTransitionEvent);
			return flow;
		}

		public static F End<F>(this F flow) where F : class, IStateFlow { RegisterFinish(flow); return flow; }

        private static void RegisterFinish(IStateFlow flow)
        {
            flow.States.Last().IsEnd = true;
        }
        private static void RegisterState(IStateFlow flow, state state)
        {
            flow.States.Add(new StateDef { State = state.Value, Caption = state.Caption });
        }

        private static void RegisterTransition(IStateFlow flow, Func<TransitionTrigger> triggerFunction, state state, Action onTransitionEvent)
        {
            flow.Transitions.Add(new TransitionDef { FromState = flow.States.Last().State, ToState = state.Value, TriggerFunction = triggerFunction, OnChanging = onTransitionEvent });
        }

        private static void RegisterTransition(IStateFlow flow, TransitionTrigger trigger, state state, Action onTransitionEvent)
        {
            flow.Transitions.Add(new TransitionDef { FromState = flow.States.Last().State, ToState = state.Value, Trigger = trigger, OnChanging = onTransitionEvent });
        }

		private static void RegisterForm(IStateFlow flow, Type form, string state = null)
		{
			flow.Forms.Add(new FormDef { FormType = form.FullName, State = state });
		}
	}
}
