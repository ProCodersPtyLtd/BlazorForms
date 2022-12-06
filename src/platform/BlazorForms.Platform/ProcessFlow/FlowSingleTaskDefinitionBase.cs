using BlazorForms.Flows.Definitions;
using BlazorForms.FlowRules;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Flows.Engine;

namespace BlazorForms.Platform.ProcessFlow
{
    [Obsolete]
    public abstract class FlowSingleTaskDefinitionBase<TSource> : FlowBase<TSource>, IFormDefinition<TSource>, ITaskRulesDefinition, IFlowForm 
            where TSource : class, IFlowModel, new()
    {
        // Flow part
        public virtual async Task UserInput()
        {
            await UserInput(GetType());
        }

        public virtual async Task UserView(Func<TSource> callback)
        {
            await UserView(GetType(), callback);
        }

        public virtual async Task UserView(Func<Task<TSource>> callback)
        {
            await UserView(GetType(), callback);
        }

        public virtual async Task UserView(Func<QueryOptions, TSource> callback)
        {
            await UserView(GetType(), callback);
        }

        public virtual async Task UserView(Func<QueryOptions, Task<TSource>> callback)
        {
            await UserView(GetType(), callback);
        }

        // Form part
        public virtual string Name { get { return this.GetType().Name; } }

        public string ProcessTaskTypeFullName => this.GetType().FullName;

        //public TSource Model { get; set; }
        public void SetModel(object model)
        {
            Model = model as TSource;
        }
    }
}
