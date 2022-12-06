using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class ModelBindingFlowReference<TSource> : IBindingFlowReference where TSource : class, IFlowModel
    {
        public string Name { get; private set; }
        public string ActionsBinding { get; private set; }
        private ModelBindingFlowReference()
        { }

        public static ModelBindingFlowReference<TSource> FromModel<TKey>(string name, Expression<Func<TSource, IEnumerable<TKey>>> items) where TKey : BindingFlowAction
        {
            var actionsBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString());
            var result = new ModelBindingFlowReference<TSource> { ActionsBinding = actionsBinding };
            return result;
        }

        public BindingFlowAction GetAction()
        {
            return new BindingFlowAction { Name = Name, ActionsBinding = ActionsBinding };
        }
    }
}
