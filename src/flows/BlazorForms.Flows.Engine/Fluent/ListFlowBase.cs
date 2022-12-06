using BlazorForms.Flows.Definitions;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Engine.Fluent
{
    public abstract class ListFlowBase<M, F> : FluentFlowBase<M>
        where M : class, IFlowModel, new()
        where F : class
    {
        public override void Define()
        {
            this.ListForm(typeof(F), LoadDataAsync, true);
        }

        public abstract Task<M> LoadDataAsync(QueryOptions queryOptions);
    }
}
