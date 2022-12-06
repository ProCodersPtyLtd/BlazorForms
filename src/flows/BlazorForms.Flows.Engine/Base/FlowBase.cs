using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Engine
{
    [Obsolete]
    public abstract class FlowBase<M> : FlowAbstract<M>
        where M : class, IFlowModel, new()
    {
        

        public override IFlowContext CreateContext()
        {
            var context = new FlowContext
            {
                Model = new M(),
                Params = new FlowParamsGeneric(),
                RefId = CreateRefId()
            };
            return context;
        }

        public virtual string CreateRefId()
        {
            return Guid.NewGuid().ToString();
        }

        public virtual void Fail(string message, ILogStreamer _logStreamer)
        {
            _logStreamer.TrackException(new FlowFailedException(message));
            throw new FlowFailedException(message);
        }

        public virtual void Begin()
        {
            Console.WriteLine($"Flow {this.GetType().Name} started.");
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task BeginAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            //Model.FlowParams = Params;
            Console.WriteLine($"Flow {this.GetType().Name} started.");
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task EndAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            Console.WriteLine($"Flow {this.GetType().Name} finished.");
        }

        //public static TableColumnBindingControlType TableColumn<TKey, TKey2>(Expression<Func<M, IEnumerable<TKey>>> items, Expression<Func<TKey, TKey2>> column)
        //{
        //    return new TableColumnBindingControlType
        //    {
        //        TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
        //        Binding = JsonPathHelper.ReplaceLambdaVar(column.Body.ToString())
        //    };
        //}

        //public static TableCountBindingControlType TableCount<TKey, TKey2>(Expression<Func<M, IEnumerable<TKey>>> items,
        //    Expression<Func<M, TKey2>> selector)
        //{
        //    return new TableCountBindingControlType
        //    {
        //        TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
        //        Binding = JsonPathHelper.ReplaceLambdaVar(selector.Body.ToString())
        //    };
        //}

        //public static TableColumnBindingControlType Table<TKey>(Expression<Func<M, IEnumerable<TKey>>> items)
        //{
        //    return new TableColumnBindingControlType
        //    {
        //        TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
        //    };
        //}

        //public static RepeaterBindingControlType Repeater<TKey>(Expression<Func<M, IEnumerable<TKey>>> items)
        //{
        //    return new RepeaterBindingControlType
        //    {
        //        TableBinding = JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
        //    };
        //}
    }
}
