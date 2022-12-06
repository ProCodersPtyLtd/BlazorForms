using BlazorForms.Flows.Definitions;
using BlazorForms.Flows;
using System.Diagnostics;

namespace BlazorFormsStateFlowDemoApp.BusinessObjects
{
    public class WorkingDaysCustomTransitionTrigger : TransitionTrigger
    {
        private int _workingDays;
        private DateTime _start;

        public WorkingDaysCustomTransitionTrigger(int workingDays)
        {
            _workingDays = workingDays;
            _start = DateTime.Now;
        }

        public override void CheckTrigger(IFlowContext context)
        {
            var days = GetBusinessDays(_start, DateTime.Now);

            if (days >= _workingDays)
            {
                Proceed = true;
            }
        }

        private static double GetBusinessDays(DateTime startD, DateTime endD)
        {
            double calcBusinessDays =
                1 + ((endD - startD).TotalDays * 5 -
                (startD.DayOfWeek - endD.DayOfWeek) * 2) / 7 - 1;

            if (endD.DayOfWeek == DayOfWeek.Saturday) calcBusinessDays--;
            if (startD.DayOfWeek == DayOfWeek.Sunday) calcBusinessDays--;

            return calcBusinessDays;
        }
    }
}
