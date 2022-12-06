using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Forms
{
    public class RepeaterFieldBuilder<TMainEntity, TProperty, TEntity> : FieldBuilder<TProperty, TEntity> 
        where TMainEntity : class
        where TEntity : class
    {
        public RepeaterFieldBuilder(string bindingProperty) : base(bindingProperty)
        {
        }

        public RepeaterFieldBuilder(DataField field) : base(field)
        {
        }

        public virtual FieldBuilder<TProperty, TEntity> Dropdown<TKey, TKey2, TKey3>(Expression<Func<TMainEntity, IEnumerable<TKey>>> items,
            Expression<Func<TKey, TKey2>> code, Expression<Func<TKey, TKey3>> name)
        {
            _field.BindingControlType = typeof(ListBindingControlType).Name;
            _field.BindingType = FieldBindingType.TableColumnSingleSelect;
            _field.ControlType = typeof(DropDown);
            _field.ViewModeControlType = typeof(DefaultDropdownReadonlyControl);

            _field.SelectEntityType = typeof(TKey);
            _field.SelectItemsProperty = items.Body.ToString().ReplaceLambdaVar();
            _field.SelectIdProperty = code.Body.ToString().ReplaceLambdaVar();
            _field.SelectNameProperty = name.Body.ToString().ReplaceLambdaVar();

            return this;
        }
    }
}
