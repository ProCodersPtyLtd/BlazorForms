using BlazorForms.Forms;
using BlazorForms.Forms.Definitions.FluentForms.Model;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BlazorForms.Forms
{
    public class FormRepeaterTypeBuilder
    {
        protected FieldBuilder _fieldBuilder;
        protected Dictionary<string, DataField> _fields = new Dictionary<string, DataField>();
        protected DataField _repeaterField;
        public string Group { get { return _repeaterField.Group; }  set { _repeaterField.Group = value; } }

        public string ItemsPath { get; protected set; }

        public FieldBuilder FieldBuilder { get { return _fieldBuilder; } }
        public IEnumerable<DataField> Fields {  get { return _fields.Values; } }
        public List<DialogButtonDetails> DialogButtons { get; private set; }  = new List<DialogButtonDetails>();
        
        public string DisplayName 
        { 
            get 
            { 
                return _repeaterField.Name; 
            } 
            set 
            { 
                _repeaterField.Name = value; 
            } 
        }

        public FormAllowAccess Access { get; set; }

        public Dictionary<string, DataField> GetFieldDictionary()
        {
            return _fields;
        }
    }

    public class FormRepeaterTypeBuilder<TMainEntity, TEntity> : FormRepeaterTypeBuilder
        where TMainEntity : class
        where TEntity : class
    {
        protected int _propertyOrder;


        public FormRepeaterTypeBuilder(Expression items, DataField repeaterField)
        {
            ItemsPath = items.ToString().ReplaceLambdaVar();
            _repeaterField = repeaterField;
        }

        public virtual FormRepeaterTypeBuilder<TMainEntity, TEntity> Confirm(ConfirmType type, string message, ConfirmButtons buttons)
        {
            _repeaterField.Confirmations.Add(new ConfirmationDetails { Type = type, Message = message, Buttons = buttons });
            return this;
        }

        public virtual RepeaterFieldBuilder<TMainEntity, TProperty, TEntity> PropertyRoot<TProperty>([NotNullAttribute] Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            _propertyOrder++;
            var bindingProperty = propertyExpression.Body.ToString().ReplaceLambdaVar();
            CheckFieldExists(typeof(TProperty), bindingProperty);
            var resultField = _fields[bindingProperty];
            resultField.Order = _propertyOrder;
            resultField.Group = Group;

            // Set binding types
            resultField.TableBindingProperty = ItemsPath;
            resultField.BindingType = FieldBindingType.TableColumn;
            resultField.BindingControlType = typeof(TableColumnBindingControlType).Name;

            // explicitly mentioned property is not hidden anymore
            resultField.Hidden = false;
            var result = new RepeaterFieldBuilder<TMainEntity, TProperty, TEntity>(resultField);
            _fieldBuilder = result;

            return result;
        }

        public virtual FieldBuilder<TProperty, TEntity> Property<TProperty>([NotNullAttribute] Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            _propertyOrder++;
            var bindingProperty = propertyExpression.Body.ToString().ReplaceLambdaVar();
            CheckFieldExists(typeof(TProperty), bindingProperty);
            var resultField = _fields[bindingProperty];
            resultField.Order = _propertyOrder;
            resultField.Group = Group;

            // Set binding types
            resultField.TableBindingProperty = ItemsPath;
            resultField.BindingType = FieldBindingType.TableColumn;
            resultField.BindingControlType = typeof(TableColumnBindingControlType).Name;

            // explicitly mentioned property is not hidden anymore
            resultField.Hidden = false;
            var result = new FieldBuilder<TProperty, TEntity>(resultField);
            _fieldBuilder = result;

            return result;
        }

        // class rule
        public virtual FormRepeaterTypeBuilder<TMainEntity, TEntity> Rule([NotNullAttribute] Type ruleType, FormRuleTriggers trigger = FormRuleTriggers.Changed)
        {
            _repeaterField.Rules.Add(new FieldRule { RuleType = ruleType, Trigger = trigger });
            return this;
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

            _fields[bindingProperty] = new DataField { Button = true, BindingProperty = bindingProperty, Label = hint, 
                BindingType = FieldBindingType.ActionButton, Group = Group };
         }

        // ToDo: add support of named format parameters like actionLink: "Form2/{id}/{id2}", where id, id2 - named page parameters
        public virtual FormRepeaterTypeBuilder<TMainEntity, TEntity> DialogButton(ButtonActionTypes actionType, string text = null, string hint = null, string actionLink = null)
        {
            DialogButtons.Add(new DialogButtonDetails { Action = actionType, Text = text, Hint = hint, LinkText = actionLink });
            return this;
        }

        public virtual FormRepeaterTypeBuilder<TMainEntity,TEntity> DialogButton(string actionLink, ButtonActionTypes actionType, string text = null, string hint = null)
        {
            DialogButtons.Add(new DialogButtonDetails { Action = actionType, Text = text, Hint = hint, LinkText = actionLink });
            return this;
        }
    }
}
