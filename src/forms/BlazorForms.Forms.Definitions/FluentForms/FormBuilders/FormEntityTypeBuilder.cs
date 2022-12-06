using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using BlazorForms.Forms.Definitions.FluentForms.Validation;
using BlazorForms.Forms.Definitions.FluentForms.FormBuilders;

namespace BlazorForms.Forms
{
    public class FormEntityTypeBuilder : FormBuilderBase
    {
        protected string _currentGroup;
        protected FieldBuilder _fieldBuilder;
        public FieldBuilder FieldBuilder { get { return _fieldBuilder; } }
        public List<DialogButtonDetails> ActionButtons { get; private set; }  = new List<DialogButtonDetails>();
        public List<FormRepeaterTypeBuilder> RepeaterBuilders { get; private set; }  = new List<FormRepeaterTypeBuilder>();
    }

    public class FormEntityTypeBuilder<TEntity> : FormEntityTypeBuilder where TEntity : class
    {
        protected int _propertyOrder;

        public FormEntityTypeBuilder()
        {
            // order of properties in the Entity has a low priority - all mentioned properties will be moved up 
            _propertyOrder = 0;
        }

        public virtual FormEntityTypeBuilder<TEntity> Group(string group)
        {
            _currentGroup = group;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> Property<TProperty>([NotNullAttribute] Expression<Func<TEntity, TProperty>> propertyExpression,
            string group = null)
        {
            _propertyOrder++;
            var bindingProperty = propertyExpression.Body.ToString().ReplaceLambdaVar();
            CheckFieldExists(typeof(TProperty), bindingProperty);
            var resultField = _fields[bindingProperty];
            resultField.Order = _propertyOrder;
            resultField.BindingType = FieldBindingType.SingleField;
            resultField.Group = group ?? _currentGroup;

            // explicitly mentioned property is not hidden anymore
            resultField.Hidden = false;
            var result = new FieldBuilder<TProperty, TEntity>(resultField);
            _fieldBuilder = result;

            return result;
        }

        public virtual FormRepeaterTypeBuilder<TEntity, TKey> Repeater<TKey>(Expression<Func<TEntity, IEnumerable<TKey>>> items, 
            [NotNullAttribute] Action<FormRepeaterTypeBuilder<TEntity, TKey>> buildAction)
            where TKey : class
        {
            _propertyOrder++;
            var bindingProperty = items.Body.ToString().ReplaceLambdaVar();
            CheckFieldExists(typeof(TEntity), bindingProperty);
            var resultField = _fields[bindingProperty];
            resultField.TableBindingProperty = bindingProperty;
            // for repeater Binding is null, only TableBinding populated
            resultField.BindingProperty = null;
            resultField.Order = _propertyOrder;
            resultField.BindingType = FieldBindingType.Repeater;
            resultField.BindingControlType = typeof(RepeaterBindingControlType).Name;
            resultField.ControlType = typeof(Repeater);

            var repeater = new FormRepeaterTypeBuilder<TEntity, TKey>(items.Body, resultField);
            repeater.Group = _currentGroup;
            RepeaterBuilders.Add(repeater);

            buildAction.Invoke(repeater);

            return repeater;
        }

        public virtual FormRepeaterTypeBuilder<TEntity, TKey> Table<TKey>(Expression<Func<TEntity, IEnumerable<TKey>>> items, 
            [NotNullAttribute] Action<FormRepeaterTypeBuilder<TEntity, TKey>> buildAction)
            where TKey : class
        {
            _propertyOrder++;
            var bindingProperty = items.Body.ToString().ReplaceLambdaVar();
            CheckFieldExists(typeof(TEntity), bindingProperty);
            var resultField = _fields[bindingProperty];
            resultField.TableBindingProperty = bindingProperty;
            resultField.BindingProperty = null;
            resultField.Order = _propertyOrder;
            resultField.BindingType = FieldBindingType.Table;
            resultField.BindingControlType = typeof(TableBindingControlType).Name;
            resultField.ControlType = typeof(Table);
            resultField.ControlTypeName = "Table";

            var table = new FormRepeaterTypeBuilder<TEntity, TKey>(items.Body, resultField);
            table.Group = _currentGroup;
            RepeaterBuilders.Add(table);

            buildAction.Invoke(table);

            return table;
        }

        private void CheckFieldExists(Type propertyType, string bindingProperty)
        {
            if (!_fields.ContainsKey(bindingProperty))
            {
                _fields[bindingProperty] = new DataField { BindingProperty = bindingProperty, DataType = propertyType };
            }
        }

        public virtual void InlineButton(string text, string hint = null)
        {
            var bindingProperty = text;
            _fields[bindingProperty] = new DataField { Button = true, BindingProperty = bindingProperty, Label = hint, BindingType = FieldBindingType.ActionButton };
         }

        // ToDo: add support of named format parameters like actionLink: "Form2/{id}/{id2}", where id, id2 - named page parameters
        public virtual FormEntityTypeBuilder<TEntity> Button(ButtonActionTypes actionType, string text = null, string hint = null, string actionLink = null)
        {
            ActionButtons.Add(new DialogButtonDetails { Action = actionType, Text = text, Hint = hint, LinkText = actionLink });
            return this;
        }

        public virtual FormEntityTypeBuilder<TEntity> Button(string actionLink, ButtonActionTypes actionType, string text = null, string hint = null)
        {
            ActionButtons.Add(new DialogButtonDetails { Action = actionType, Text = text, Hint = hint, LinkText = actionLink });
            return this;
        }

        public virtual FormEntityTypeBuilder<TEntity> Rule([NotNullAttribute] Type ruleType, FormRuleTriggers trigger = FormRuleTriggers.Loaded)
        {
            _formField.Rules.Add(new FieldRule { RuleType = ruleType, Trigger = trigger });
            return this;
        }

        public void AssertValid()
        {
            AssertValid<TEntity>();
        }
    }
}
