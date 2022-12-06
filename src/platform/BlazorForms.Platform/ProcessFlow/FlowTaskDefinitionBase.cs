using BlazorForms.Flows.Definitions;
using BlazorForms.FlowRules;
using BlazorForms.Forms;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace BlazorForms.Platform.ProcessFlow
{
    public abstract class FlowTaskDefinitionBase<TSource> : BindingModelAbstract<TSource>, IFormDefinition<TSource>, ITaskRulesDefinition, IFlowForm where TSource : class
    {
        public virtual string Name { get { return this.GetType().Name; } }

        public string ProcessTaskTypeFullName => this.GetType().FullName;

        public TSource Model { get; set; }

        public void SetModel(object model)
        {
            Model = model as TSource;
        }

        //public static IBindingControlType ModelProp<TKey>(Expression<Func<TSource, TKey>> selector)
        //{
        //    var selectorString = selector.Body.ToString();
        //    selectorString = JsonPathHelper.ReplaceLambdaVar(selectorString);
        //    return new BindingControlType { Binding = selectorString };
        //}

        //public static ListBindingControlType SingleSelect<TSelected, TKey, TKey2, TKey3>(Expression<Func<TSource, TSelected>> selectedId,
        //    Expression<Func<TSource, IEnumerable<TKey>>> items,
        //    Expression<Func<TKey, TKey2>> id,
        //    Expression<Func<TKey, TKey3>> name)
        //{
        //    var control = new ListBindingControlType
        //    {
        //        Binding = JsonPathHelper.ReplaceLambdaVar(selectedId.Body.ToString()),
        //        ItemsBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
        //        IdBinding = JsonPathHelper.ReplaceLambdaVar(id.Body.ToString()),
        //        NameBinding = JsonPathHelper.ReplaceLambdaVar(name.Body.ToString())
        //    };

        //    return control;
        //}

        //public static TableColumnBindingControlType Table<TKey>(Expression<Func<TSource, IEnumerable<TKey>>> items)
        //{
        //    return new TableColumnBindingControlType
        //    {
        //        TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
        //    };
        //}

        //public static RepeaterBindingControlType Repeater<TKey>(Expression<Func<TSource, IEnumerable<TKey>>> items)
        //{
        //    return new RepeaterBindingControlType
        //    {
        //        TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
        //    };
        //}

        //public static SelectableListBindingControlType SelectableList<TKey, TKey2>(Expression<Func<TSource, IEnumerable<TKey>>> items, Expression<Func<TSource, TKey2>> targetProperty)
        //{
        //    return new SelectableListBindingControlType
        //    {
        //        Binding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
        //        TargetBinding = JsonPathHelper.ReplaceLambdaVar(targetProperty.Body.ToString())
        //    };
        //}

        //public static TableColumnBindingControlType TableColumn<TKey, TKey2>(Expression<Func<TSource, IEnumerable<TKey>>> items,
        //    Expression<Func<TKey, TKey2>> column)
        //{
        //    return new TableColumnBindingControlType
        //    {
        //        TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
        //        Binding = JsonPathHelper.ReplaceLambdaVar(column.Body.ToString())
        //    };
        //}
        //public static TableColumnBindingControlType TableFooter<TKey, TKey2>(Expression<Func<TSource, IEnumerable<TKey>>> items,
        //    Expression<Func<TKey, TKey2>> column)
        //{
        //    // ToDo: copypaste, please implement properly
        //    return new TableColumnBindingControlType
        //    {
        //        TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
        //        Binding = JsonPathHelper.ReplaceLambdaVar(column.Body.ToString())
        //    };
        //}

        //public static TableCountBindingControlType TableCount<TKey, TKey2>(Expression<Func<TSource, IEnumerable<TKey>>> items,
        //    Expression<Func<TSource, TKey2>> selector)
        //{
        //    return new TableCountBindingControlType
        //    {
        //        TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
        //        Binding = JsonPathHelper.ReplaceLambdaVar(selector.Body.ToString())
        //    };
        //}
    }
}
