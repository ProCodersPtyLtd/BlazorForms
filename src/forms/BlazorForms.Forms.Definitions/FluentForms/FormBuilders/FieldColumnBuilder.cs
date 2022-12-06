using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;

namespace BlazorForms.Forms
{
    public class FieldColumnBuilder
    {
        protected DataField _field;
        public DataField Field { get { return _field; } }
    }

    public class FieldColumnBuilder<TProperty, TEntity> : FieldColumnBuilder where TEntity : class
    {

        public FieldColumnBuilder(string bindingProperty)
        {
            _field = new DataField { BindingProperty = bindingProperty, DataType = typeof(TProperty) };
        }

        public FieldColumnBuilder(DataField field)
        {
            _field = field;
        }

        // class rule
        public virtual FieldColumnBuilder<TProperty, TEntity> Rule([NotNullAttribute] Type ruleType, FormRuleTriggers trigger = FormRuleTriggers.Changed)
        {
            _field.Rules.Add(new FieldRule { RuleType = ruleType, Trigger = trigger });
            return this;
        }

        public virtual FieldColumnBuilder<TProperty, TEntity> IsRequired(bool required = true)
        {
            _field.Required = required;
            return this;
        }

        public virtual FieldColumnBuilder<TProperty, TEntity> IsHidden(bool hidden = true)
        {
            _field.Hidden = hidden;
            return this;
        }

        public virtual FieldColumnBuilder<TProperty, TEntity> IsPrimaryKey(bool pk = true)
        {
            _field.PrimaryKey = pk;
            return this;
        }

        public virtual FieldColumnBuilder<TProperty, TEntity> IsReadOnly(bool readOnly = true)
        {
            _field.ReadOnly = readOnly;
            return this;
        }
        public virtual FieldColumnBuilder<TProperty, TEntity> IsHighlighted(bool b = true)
        {
            _field.Highlighted = b;
            return this;
        }
        public virtual FieldColumnBuilder<TProperty, TEntity> IsPassword(bool b = true)
        {
            _field.Password = b;
            return this;
        }
        public virtual FieldColumnBuilder<TProperty, TEntity> NoCaption(bool b = true)
        {
            _field.NoCaption = b;
            return this;
        }

        public virtual FieldColumnBuilder<TProperty, TEntity> IsUnique(bool unique = true)
        {
            _field.Unique = unique;
            return this;
        }

        public virtual FieldColumnBuilder<TProperty, TEntity> Label(string label)
        {
            _field.Label = label;
            return this;
        }

        public virtual FieldColumnBuilder<TProperty, TEntity> Hint(string s)
        {
            _field.Hint = s;
            return this;
        }

        public virtual FieldColumnBuilder<TProperty, TEntity> Name(string name)
        {
            _field.Name = name;
            return this;
        }

        public virtual FieldColumnBuilder<TProperty, TEntity> Filter(FieldFilterType type)
        {
            _field.FilterType = type;
            return this;
        }
        public virtual FieldColumnBuilder<TProperty, TEntity> FilterRefField(string s)
        {
            _field.FilterRefField = s;
            return this;
        }

        public virtual FieldColumnBuilder<TProperty, TEntity> Format(string format)
        {
            _field.Format = format;
            return this;
        }

        public virtual FieldColumnBuilder<TProperty, TEntity> Control(Type controlType)
        {
            _field.ControlType = controlType;
            return this;
        }

        public virtual FieldColumnBuilder<TProperty, TEntity> ControlReadOnly(Type controlType)
        {
            _field.ViewModeControlType = controlType;
            return this;
        }

        public virtual DropdownFieldColumnBuilder<TEntity2> Dropdown<TEntity2>()
        {
            return new DropdownFieldColumnBuilder<TEntity2>(this, _field);
        }

        public class DropdownFieldColumnBuilder<TEntity2>
        {
            private FieldColumnBuilder<TProperty, TEntity> _parent;
            private DataField _field;

            public DropdownFieldColumnBuilder(FieldColumnBuilder<TProperty, TEntity> parent, DataField field)
            {
                _field = field;
                _parent = parent;
            }

            public FieldColumnBuilder<TProperty, TEntity> Set<TKey2, TKey3>(
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
