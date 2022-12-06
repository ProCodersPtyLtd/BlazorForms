using BlazorForms.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;

namespace BlazorForms.Forms
{
    public class FormListBuilder
    {
        protected FormListTypeBuilder _builder;
        public FormListTypeBuilder Builder { get { return _builder; } }
        public Type ListType { get; protected set; }
    }

    public class FormListBuilder<M> : FormListBuilder
        where M: class
    { 
        public virtual FormListBuilder List<TKey>(Expression<Func<M, IEnumerable<TKey>>> items, [NotNullAttribute] Action<FormListTypeBuilder<TKey>> buildAction)
            where TKey: class
        {
            var builder = new FormListTypeBuilder<TKey>(items.Body);
            _builder = builder;
            buildAction.Invoke(builder);
            ListType = typeof(TKey);
            return this;
        }
    }
}
