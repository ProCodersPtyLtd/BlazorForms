using BlazorForms.Flows.Definitions;
using BlazorForms.Flows;

namespace BlazorFormsStateFlowDemoApp.BusinessObjects
{
    public abstract class DocumentFlowBase<M> : StateFlowBase<M>
        where M : class, IFlowModel
    {
        public state New;
        public state Assigned;
        public state Reviewing;
        public state Alarmed;
        public state Closed;

        public status Open;
        public status Stale;
        public status StatusClosed = new status("Closed");

    }
}
