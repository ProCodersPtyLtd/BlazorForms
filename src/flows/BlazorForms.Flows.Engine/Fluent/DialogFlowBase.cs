using BlazorForms.Flows.Definitions;
using System.Threading.Tasks;

namespace BlazorForms.Flows
{
    public abstract class DialogFlowBase<M, F> : FluentFlowBase<M>
        where M : class, IFlowModel, new()
        where F : class
    {
        public override void Define()
        {
            this
                .Begin()
                .Next(LoadDataAsync)
                .NextForm(typeof(F))
                .Next(SaveDataAsync)
                .End();
        }

        protected int GetId()
        {
            var strid = Params["Id"];
            int id;

            if (strid != null && int.TryParse(strid, out id))
            {
                return id;
            }

            return 0;
        }

        public abstract Task LoadDataAsync();
        public abstract Task SaveDataAsync();
    }
}
