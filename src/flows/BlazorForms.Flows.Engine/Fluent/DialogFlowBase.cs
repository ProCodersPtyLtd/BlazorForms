using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public abstract Task LoadDataAsync();
        public abstract Task SaveDataAsync();
    }
}
