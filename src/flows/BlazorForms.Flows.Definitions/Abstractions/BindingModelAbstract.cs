using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class BindingModelAbstract<TSource> where TSource : class
    {
        public static FieldBinding CustomModelProp(string selectorString)
        {
            return new FieldBinding
            {
                Binding = selectorString,
                BindingControlType = typeof(BindingControlType).Name,
                BindingType = FieldBindingType.SingleField
            };
        }

        public static FieldBinding ModelProp<TKey>(Expression<Func<TSource, TKey>> selector)
        {
            var selectorString = selector.Body.ToString();
            selectorString = JsonPathHelper.ReplaceLambdaVar(selectorString);
            return new FieldBinding
            {
                Binding = selectorString,
                BindingControlType = typeof(BindingControlType).Name,
                BindingType = FieldBindingType.SingleField
            };
        }

        public static FieldBinding ActionButton(string binding, ActionType actionType)
        {
            return new FieldBinding
            {
                Binding = binding,
                ActionType = actionType,
                BindingType = FieldBindingType.ActionButton
            };
        }

        private static string GetBindingByActionType(ActionType actionType)
        {
            return actionType switch
            {
                ActionType.Reject => ModelBinding.RejectButtonBinding,
                ActionType.Submit => ModelBinding.SubmitButtonBinding,
                ActionType.Save => ModelBinding.SaveButtonBinding,
                ActionType.Close => ModelBinding.CloseButtonBinding,
                ActionType.Cancel => ModelBinding.CloseButtonBinding,
                ActionType.CloseFinish => ModelBinding.CloseFinishButtonBinding,
                ActionType.SubmitClose => ModelBinding.SubmitCloseButtonBinding,
                _ => throw new Exception("ActionType binding is not found"),
            };
        }

        public static FieldBinding EditWithOptions<TSelected, TKey, TKey2>(Expression<Func<TSource, TSelected>> selector,
            Expression<Func<TSource, IEnumerable<TKey>>> items,
            Expression<Func<TKey, TKey2>> name)
        {
            var control = new FieldBinding
            {
                Binding = JsonPathHelper.ReplaceLambdaVar(selector.Body.ToString()),
                ItemsBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
                NameBinding = JsonPathHelper.ReplaceLambdaVar(name.Body.ToString()),
                BindingControlType = typeof(ListBindingControlType).Name,
                BindingType = FieldBindingType.SingleField
            };

            return control;
        }

        public static FieldBinding SingleSelect<TSelected, TKey, TKey2, TKey3>(Expression<Func<TSource, TSelected>> selectedId,
            Expression<Func<TSource, IEnumerable<TKey>>> items,
            Expression<Func<TKey, TKey2>> id,
            Expression<Func<TKey, TKey3>> name)
        {
            var control = new FieldBinding
            {
                Binding = JsonPathHelper.ReplaceLambdaVar(selectedId.Body.ToString()),
                ItemsBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
                IdBinding = JsonPathHelper.ReplaceLambdaVar(id.Body.ToString()),
                NameBinding = JsonPathHelper.ReplaceLambdaVar(name.Body.ToString()),
                BindingControlType = typeof(ListBindingControlType).Name,
                BindingType = FieldBindingType.SingleSelect
            };

            return control;
        }

        public static FieldBinding TableColumn<TKey, TKey2>(Expression<Func<TSource, IEnumerable<TKey>>> items, Expression<Func<TKey, TKey2>> column)
        {
            return new FieldBinding
            {
                TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
                Binding = JsonPathHelper.ReplaceLambdaVar(column.Body.ToString()),
                BindingControlType = typeof(TableColumnBindingControlType).Name,
                BindingType = FieldBindingType.TableColumn
            };
        }

        public static FieldBinding TableColumn<TKey>(Expression<Func<TSource, IEnumerable<TKey>>> items)
        {
            return new FieldBinding
            {
                TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
                Binding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
                BindingControlType = typeof(TableColumnBindingControlType).Name,
                BindingType = FieldBindingType.TableColumn
            };
        }

        public static FieldBinding TableColumnSingleSelect<TKey, TKey2, TKey3, TKey4, TKey5>(Expression<Func<TSource, IEnumerable<TKey>>> tableItems, Expression<Func<TKey, TKey2>> column,
            Expression<Func<TSource, IEnumerable<TKey3>>> items,
            Expression<Func<TKey3, TKey4>> id,
            Expression<Func<TKey3, TKey5>> name)
        {
            return new FieldBinding
            {
                TableBinding = JsonPathHelper.ReplaceLambdaVar(tableItems.Body.ToString()),
                Binding = JsonPathHelper.ReplaceLambdaVar(column.Body.ToString()),
                BindingControlType = typeof(TableColumnBindingControlType).Name,
                BindingType = FieldBindingType.TableColumnSingleSelect,

                ItemsBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
                IdBinding = JsonPathHelper.ReplaceLambdaVar(id.Body.ToString()),
                NameBinding = JsonPathHelper.ReplaceLambdaVar(name.Body.ToString()),
            };
        }

        public static FieldBinding TableFooter<TKey, TKey2>(Expression<Func<TSource, IEnumerable<TKey>>> items,
            Expression<Func<TKey, TKey2>> column)
        {
            // ToDo: copypaste, please implement properly
            return new FieldBinding
            {
                TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
                Binding = JsonPathHelper.ReplaceLambdaVar(column.Body.ToString()),
                BindingType = FieldBindingType.TableFooter
            };
        }

        public static FieldBinding TableCount<TKey, TKey2>(Expression<Func<TSource, IEnumerable<TKey>>> items,
            Expression<Func<TSource, TKey2>> selector)
        {
            return new FieldBinding
            {
                TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
                Binding = JsonPathHelper.ReplaceLambdaVar(selector.Body.ToString()),
                BindingControlType = typeof(TableCountBindingControlType).Name,
                BindingType = FieldBindingType.TableCount
            };
        }

        public static FieldBinding Table<TKey>(Expression<Func<TSource, IEnumerable<TKey>>> items)
        {
            return new FieldBinding
            {
                TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
                BindingControlType = typeof(TableBindingControlType).Name,
                BindingType = FieldBindingType.Table
            };
        }

        public static FieldBinding Repeater<TKey>(Expression<Func<TSource, IEnumerable<TKey>>> items, RepeaterParameters parameters = null)
        {
            return new FieldBinding
            {
                TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
                BindingControlType = typeof(RepeaterBindingControlType).Name,
                BindingType = FieldBindingType.Repeater,
                Parameters = parameters
            };
        }

        public static FieldBinding ActionButton(ActionType actionType)
        {
            return new FieldBinding
            {
                Binding = GetBindingByActionType(actionType),
                ActionType = actionType,
                BindingType = FieldBindingType.ActionButton
            };
        }

        public static FieldBinding FlowReferenceButtons(params IBindingFlowReference[] actions)
        {
            return new FieldBinding
            {
                Binding = ModelBinding.FlowReferenceButtonsBinding,
                BindingType = FieldBindingType.FlowReferenceButtons,
                ContextMenuActions = actions.Select(a => a.GetAction()).ToList()
            };
        }

        public static FieldBinding TableColumnContextMenu<TKey>(Expression<Func<TSource, IEnumerable<TKey>>> items, params IBindingFlowReference[] actions)
        {
            return new FieldBinding
            {
                TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
                BindingType = FieldBindingType.TableColumnContextMenu,
                ContextMenuActions = actions.Select(a => a.GetAction()).ToList()
            };
        }

        public static FieldBinding ListFormContextMenu(params IBindingFlowReference[] actions)
        {
            return new FieldBinding
            {
                Binding = ModelBinding.ListFormContextMenuBinding,
                BindingType = FieldBindingType.ListFormContextMenu,
                ContextMenuActions = actions.Select(a => a.GetAction()).ToList()
            };
        }

        public static FieldBinding SelectableList<TKey, TKey2>(Expression<Func<TSource, IEnumerable<TKey>>> items, Expression<Func<TSource, TKey2>> targetProperty)
        {
            return new FieldBinding
            {
                Binding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
                TargetBinding = JsonPathHelper.ReplaceLambdaVar(targetProperty.Body.ToString()),
                BindingControlType = typeof(SelectableListBindingControlType).Name,
                BindingType = FieldBindingType.SelectableList
            };
        }
    }
}
