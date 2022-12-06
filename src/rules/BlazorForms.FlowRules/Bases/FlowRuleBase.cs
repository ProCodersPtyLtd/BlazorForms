using BlazorForms.FlowRules;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.FlowRules
{
    public abstract class FlowRuleBase<M> : IFlowRule<M> where M : class
    {
        public abstract string RuleCode { get; }
        public virtual RuleTypes RuleType { get; } = RuleTypes.ValidationRule;
        public RuleExecutionResult Result { get; internal set; } = new RuleExecutionResult();

        protected RuleExecutionParameters RunParams;
        private ILogStreamer _logStreamer;

        public abstract void Execute(M model);

        public void ExecuteUntyped(object model)
        {
            Execute(model as M);
        }

        public Task ExecuteUntypedAsync(object model, ILogStreamer logStreamer)
        {
            _logStreamer = logStreamer;
            _logStreamer.TrackException(new NotImplementedException("ExecuteUntyped shoule be executed instead"));
            throw new NotImplementedException("ExecuteUntyped shoule be executed instead");
        }

        public void Initialize(RuleExecutionParameters parameters)
        {
            RunParams = parameters;
            Result = new RuleExecutionResult();
        }

        public static string SingleField<TKey>(Expression<Func<M, TKey>> selector)
        {
            var selectorString = selector.Body.ToString();
            selectorString = JsonPathHelper.ReplaceLambdaVar(selectorString);
            return selectorString;
        }

        public static string RowField<TKey, TKey2>(Expression<Func<M, IEnumerable<TKey>>> items, Expression<Func<TKey, TKey2>> column, int rowIndex)
        {
            var field = new FieldBinding
            {
                TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
                Binding = JsonPathHelper.ReplaceLambdaVar(column.Body.ToString()),
                BindingControlType = typeof(TableColumnBindingControlType).Name,
                BindingType = FieldBindingType.TableColumn
            };

            field.ResolveKey(new FieldBindingArgs { RowIndex = rowIndex });
            return field.Key;
        }
    }
}
