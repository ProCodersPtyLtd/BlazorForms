using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace BlazorForms.Forms
{
    public class FieldBuilder
    {
        protected DataField _field;
        public DataField Field { get { return _field; } }
    }

    public class FieldBuilder<TProperty, TEntity> : FieldBuilder where TEntity : class
    {

        public FieldBuilder(string bindingProperty)
        {
            _field = new DataField { BindingProperty = bindingProperty, DataType = typeof(TProperty) };
        }

        public FieldBuilder(DataField field)
        {
            _field = field;
        }

        // ToDo: add notation to specify Group only once for all following controls
        public virtual FieldBuilder<TProperty, TEntity> Group(string group)
        {
            _field.Group = group;
            return this;
        }


        // class rule
        public virtual FieldBuilder<TProperty, TEntity> Rule([NotNullAttribute] Type ruleType, FormRuleTriggers trigger = FormRuleTriggers.Changed, 
            bool isOuterProperty = false)
        {
            _field.Rules.Add(new FieldRule { RuleType = ruleType, Trigger = trigger, IsOuterProperty = isOuterProperty });
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> IsRequired(bool required = true)
        {
            _field.Required = required;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> IsHidden(bool hidden = true)
        {
            _field.Hidden = hidden;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> IsPrimaryKey(bool pk = true)
        {
            _field.PrimaryKey = pk;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> IsReadOnly(bool readOnly = true)
        {
            _field.ReadOnly = readOnly;
            return this;
        }
        public virtual FieldBuilder<TProperty, TEntity> IsHighlighted(bool b = true)
        {
            _field.Highlighted = b;
            return this;
        }
        public virtual FieldBuilder<TProperty, TEntity> IsPassword(bool b = true)
        {
            _field.Password = b;
            return this;
        }
        public virtual FieldBuilder<TProperty, TEntity> NoCaption(bool b = true)
        {
            _field.NoCaption = b;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> IsUnique(bool unique = true)
        {
            _field.Unique = unique;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> Label(string label)
        {
            _field.Label = label;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> Hint(string s)
        {
            _field.Hint = s;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> Name(string name)
        {
            _field.Name = name;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> Filter(FieldFilterType type)
        {
            _field.FilterType = type;
            return this;
        }
        public virtual FieldBuilder<TProperty, TEntity> FilterRefField(string s)
        {
            _field.FilterRefField = s;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> Format(string format)
        {
            _field.Format = format;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> Control(ControlType controlType)
        {
            _field.ControlTypeName = controlType.ToString();
            return this;
        }
        public virtual FieldBuilder<TProperty, TEntity> Control(Type controlType)
        {
            _field.ControlType = controlType;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> ControlReadOnly(Type controlType)
        {
            _field.ViewModeControlType = controlType;
            return this;
        }
        public virtual FieldBuilder<TProperty, TEntity> EditWithOptions<TKey, TKey2>(Expression<Func<TEntity, IEnumerable<TKey>>> items, 
            Expression<Func<TKey, TKey2>> name)
        {
            _field.BindingControlType = typeof(ListBindingControlType).Name;
            _field.BindingType = FieldBindingType.SingleField;
            _field.ControlType = typeof(Autocomplete);
            //_field.ViewModeControlType = typeof(DefaultDropdownReadonlyControl);

            _field.SelectEntityType = typeof(TKey);
            _field.SelectItemsProperty = items.Body.ToString().ReplaceLambdaVar();
            _field.SelectNameProperty = name.Body.ToString().ReplaceLambdaVar();

            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> Dropdown<TKey, TKey2, TKey3>(Expression<Func<TEntity, IEnumerable<TKey>>> items,
            Expression<Func<TKey, TKey2>> code, Expression<Func<TKey, TKey3>> name)
        {
            _field.BindingControlType = typeof(ListBindingControlType).Name;
            _field.BindingType = FieldBindingType.SingleSelect;
            _field.ControlType = typeof(DropDown);
            _field.ViewModeControlType = typeof(DefaultDropdownReadonlyControl);

            _field.SelectEntityType = typeof(TKey);
            _field.SelectItemsProperty = items.Body.ToString().ReplaceLambdaVar();
            _field.SelectIdProperty = code.Body.ToString().ReplaceLambdaVar();
            _field.SelectNameProperty = name.Body.ToString().ReplaceLambdaVar();

            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> DropdownSearch<TKey, TKey2, TKey3>(Expression<Func<TEntity, IEnumerable<TKey>>> items,
            Expression<Func<TKey, TKey2>> code, Expression<Func<TKey, TKey3>> name)
        {
            _field.BindingControlType = typeof(ListBindingControlType).Name;
            _field.BindingType = FieldBindingType.SingleField;
            _field.ControlType = typeof(DropDownSearch);
            _field.ViewModeControlType = typeof(DefaultDropdownReadonlyControl);

            _field.SelectEntityType = typeof(TKey);
            _field.SelectItemsProperty = items.Body.ToString().ReplaceLambdaVar();
            _field.SelectIdProperty = code.Body.ToString().ReplaceLambdaVar();
            _field.SelectNameProperty = name.Body.ToString().ReplaceLambdaVar();

            return this;
        }

        // old drop down
        public virtual DropdownFieldBuilder<TEntity2> Dropdown<TEntity2>()
        {
            return new DropdownFieldBuilder<TEntity2>(this, _field);
        }

        public class DropdownFieldBuilder<TEntity2>
        {
            private FieldBuilder<TProperty, TEntity> _parent;
            private DataField _field;

            public DropdownFieldBuilder(FieldBuilder<TProperty, TEntity> parent, DataField field)
            {
                _field = field;
                _parent = parent;
            }

            public FieldBuilder<TProperty, TEntity> Set<TKey2, TKey3>(
                [NotNullAttribute] Expression<Func<TEntity2, TKey2>> id,
                [NotNullAttribute] Expression<Func<TEntity2, TKey3>> name)
            {
                _field.ControlType = typeof(DefaultDropdownControl);
                _field.ViewModeControlType = typeof(DefaultDropdownReadonlyControl);
                
                _field.SelectEntityType = typeof(TEntity2);
                _field.SelectIdProperty = id.Body.ToString().ReplaceLambdaVar();
                _field.SelectNameProperty = name.Body.ToString().ReplaceLambdaVar();

                return _parent;
            }
        }
    }
}
