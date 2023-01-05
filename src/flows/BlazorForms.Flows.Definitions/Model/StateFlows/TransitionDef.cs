using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Definitions
{
	public class TransitionDef
	{
		public string FormType { get; set; }
		public string FromState { get; set; }
		public string ToState { get; set; }
		public TransitionTrigger Trigger { get; set; }
		public Func<TransitionTrigger> TriggerFunction { get; set; }
		public Action OnChanging { get; set; }
		public Func<Task> OnChangingAsync { get; set; }

		public TransitionTrigger GetTrigger()
		{
			return Trigger ?? TriggerFunction();
		}

		public bool IsButtonTrigger()
		{
			var trigger = GetTrigger();
			return trigger is ButtonTransitionTrigger;
		}

		public bool IsUserActionTrigger()
		{
			var trigger = GetTrigger();
			return trigger is UserActionTransitionTrigger;
		}
	}
}
