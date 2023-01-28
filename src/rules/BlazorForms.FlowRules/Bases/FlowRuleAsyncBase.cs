using BlazorForms.FlowRules;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.FlowRules
{
    public abstract class FlowRuleAsyncAbstract<M> : IFlowRuleAsync<M> where M : class
    {
        public abstract string RuleCode { get; }
        public virtual RuleTypes RuleType { get; } = RuleTypes.ValidationRule;
        public RuleExecutionResult Result { get; internal set; } = new RuleExecutionResult();

        protected RuleExecutionParameters RunParams;

        public abstract Task Execute(M model);
        public abstract void ExecuteUntyped(object model);
        public abstract Task ExecuteUntypedAsync(object model, ILogStreamer logStreamer);

        public void Initialize(RuleExecutionParameters parameters)
        {
            RunParams = parameters;
            Result = new RuleExecutionResult();
        }
    }

    public abstract class FlowRuleAsyncBase<M> : FlowRuleAsyncAbstract<M> where M : class
    {
        public sealed override void ExecuteUntyped(object model)
        {
            throw new NotImplementedException("ExecuteUntypedAsync shoule be executed instead");
        }

        public sealed override async Task ExecuteUntypedAsync(object model, ILogStreamer logStreamer)
        {
            await Execute(model as M);
        }

        public string FindField<TKey>(Expression<Func<M, TKey>> selector, string propertyBinding, int rowIndex = -1 )
        {
            var selectorString = selector.Body.ToString();
            selectorString = JsonPathHelper.ReplaceLambdaVar(selectorString);

            // assemble binding
            var suff = rowIndex >= 0 ? $"[{rowIndex}]" : "";
            var end = propertyBinding?.Replace("$", "");
            selectorString = $"{selectorString}{suff}{end}";
            return selectorString;
        }

        public void Trigger(string selector, FormRuleTriggers triggerType = FormRuleTriggers.Changed)
        {
            Result.ChangedFields.Add(selector);
        }

        public string SingleField<TKey>(Expression<Func<M, TKey>> selector)
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

        public string ModelProp<TKey>(Expression<Func<M, TKey>> selector)
        {
            var selectorString = selector.Body.ToString();
            selectorString = JsonPathHelper.ReplaceLambdaVar(selectorString);
            return selectorString;
        }
    }

    public interface IAccessRuleAsyncBase
    {
        AccessRuleModel AccessModel { get; set; }
    }

    public abstract class AccessRuleAsyncBase<M> : FlowRuleAsyncBase<M>, IAccessRuleAsyncBase where M : class
    {
        public AccessRuleModel AccessModel { get; set; }

        public override string RuleCode => this.GetType().Name;
        public override RuleTypes RuleType { get { return RuleTypes.AccessRule; } }

        public async override Task Execute(M model)
        {
            await Execute(model, AccessModel);
        }

        public abstract Task Execute(M model, AccessRuleModel accessModel);

    }

    
}
