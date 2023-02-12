﻿using BlazorForms.Forms;
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
    public class FormCardListTypeBuilder<TMainEntity, TEntity> : FormRepeaterTypeBuilder
        where TMainEntity : class
        where TEntity : class
    {
        protected int _propertyOrder;


        public FormCardListTypeBuilder(Expression items, DataField repeaterField)
        {
            ItemsPath = items.ToString().ReplaceLambdaVar();
            _repeaterField = repeaterField;
        }

        public virtual FormCardListTypeBuilder<TMainEntity, TEntity> Confirm(ConfirmType type, string message, ConfirmButtons buttons)
        {
            _repeaterField.Confirmations.Add(new ConfirmationDetails { Type = type, Message = message, Buttons = buttons });
            return this;
        }

        public virtual FormCardListTypeBuilder<TMainEntity, TEntity> Button(ButtonActionTypes actionType, string text = null, string hint = null,
            string actionLink = null)
        {
            _propertyOrder++;
            var property = GetButtonBinding(actionType);
            var fieldSetGroup = ItemsPath;
            CreateFieldIfNotExists(typeof(TEntity), property);
            var resultField = _fields[property];
            resultField.ControlTypeName = ControlType.CardButton.ToString();
            resultField.Order = _propertyOrder;
            resultField.TableBindingProperty = ItemsPath;
            resultField.BindingType = FieldBindingType.RepeaterActionButton;
            resultField.BindingControlType = typeof(TableColumnBindingControlType).Name;
            resultField.Hidden = false;
            resultField.FieldSetGroup = fieldSetGroup;
            resultField.ActionLink = actionLink;
            resultField.Label = text ?? actionType.ToString();
            resultField.Hint = hint;
            return this;
        }

        public virtual FormCardListTypeBuilder<TMainEntity, TEntity> Rule([NotNullAttribute] Type ruleType, 
            FormRuleTriggers trigger = FormRuleTriggers.ItemChanged)
        {
            _repeaterField.Rules.Add(new FieldRule { RuleType = ruleType, Trigger = trigger });
            return this;
        }

        public virtual void Card<TKey, TKey2>([NotNull] Expression<Func<TEntity, TKey>> title, [NotNull] Expression<Func<TEntity, TKey2>> body)
        {
            Card<TKey, TKey2, object>(title, body, null);
        }


        public virtual void Card<TKey, TKey2, TKey3>([NotNull] Expression<Func<TEntity, TKey>> title, [NotNull] Expression<Func<TEntity, TKey2>> body, 
            Expression<Func<TEntity, TKey3>> avatar)
        {
            //_field.ControlTypeName = ControlType.Card.ToString(); ;
            //return this;
            _propertyOrder++;
            DataField resultField;
            string property;

            property = title.Body.ToString().ReplaceLambdaVar();
            //var fieldSetGroup = property;
            var fieldSetGroup = ItemsPath;
            CreateFieldIfNotExists(typeof(TKey), property);
            resultField = _fields[property];
            resultField.ControlTypeName = ControlType.CardTitle.ToString();
            resultField.Order = _propertyOrder;
            resultField.TableBindingProperty = ItemsPath;
            resultField.BindingType = FieldBindingType.ListCard;
            resultField.BindingControlType = typeof(TableColumnBindingControlType).Name;
            resultField.Hidden = false;
            resultField.FieldSetGroup = fieldSetGroup;

            property = body.Body.ToString().ReplaceLambdaVar();
            CreateFieldIfNotExists(typeof(TKey2), property);
            resultField = _fields[property];
            resultField.ControlTypeName = ControlType.CardBody.ToString();
            resultField.Order = _propertyOrder;
            resultField.TableBindingProperty = ItemsPath;
            resultField.BindingType = FieldBindingType.ListCard;
            resultField.BindingControlType = typeof(TableColumnBindingControlType).Name;
            resultField.Hidden = false;
            resultField.FieldSetGroup = fieldSetGroup;

            if (avatar != null)
            {
                property = avatar.Body.ToString().ReplaceLambdaVar();
                CreateFieldIfNotExists(typeof(TKey3), property);
                resultField = _fields[property];
                resultField.ControlTypeName = ControlType.CardAvatar.ToString();
                resultField.Order = _propertyOrder;
                resultField.TableBindingProperty = ItemsPath;
                resultField.BindingType = FieldBindingType.ListCard;
                resultField.BindingControlType = typeof(TableColumnBindingControlType).Name;
                resultField.Hidden = false;
                resultField.FieldSetGroup = fieldSetGroup;
            }
        }

        private void CreateFieldIfNotExists(Type propertyType, string bindingProperty)
        {
            if (!_fields.ContainsKey(bindingProperty))
            {
                _fields[bindingProperty] = new DataField { BindingProperty = bindingProperty, DataType = propertyType };
            }
        }

        //public virtual RepeaterFieldBuilder<TMainEntity, TProperty, TEntity> PropertyRoot<TProperty>([NotNullAttribute] Expression<Func<TEntity, TProperty>> propertyExpression)
        //{
        //    _propertyOrder++;
        //    var bindingProperty = propertyExpression.Body.ToString().ReplaceLambdaVar();
        //    CreateFieldIfNotExists(typeof(TProperty), bindingProperty);
        //    var resultField = _fields[bindingProperty];
        //    resultField.Order = _propertyOrder;
        //    resultField.Group = Group;

        //    // Set binding types
        //    resultField.TableBindingProperty = ItemsPath;
        //    resultField.BindingType = FieldBindingType.TableColumn;
        //    resultField.BindingControlType = typeof(TableColumnBindingControlType).Name;

        //    // explicitly mentioned property is not hidden anymore
        //    resultField.Hidden = false;
        //    var result = new RepeaterFieldBuilder<TMainEntity, TProperty, TEntity>(resultField);
        //    _fieldBuilder = result;

        //    return result;
        //}

        //public virtual FieldBuilder<TProperty, TEntity> Property<TProperty>([NotNullAttribute] Expression<Func<TEntity, TProperty>> propertyExpression)
        //{
        //    _propertyOrder++;
        //    var bindingProperty = propertyExpression.Body.ToString().ReplaceLambdaVar();
        //    CreateFieldIfNotExists(typeof(TProperty), bindingProperty);
        //    var resultField = _fields[bindingProperty];
        //    resultField.Order = _propertyOrder;
        //    resultField.Group = Group;

        //    // Set binding types
        //    resultField.TableBindingProperty = ItemsPath;
        //    resultField.BindingType = FieldBindingType.TableColumn;
        //    resultField.BindingControlType = typeof(TableColumnBindingControlType).Name;

        //    // explicitly mentioned property is not hidden anymore
        //    resultField.Hidden = false;
        //    var result = new FieldBuilder<TProperty, TEntity>(resultField);
        //    _fieldBuilder = result;

        //    return result;
        //}

        //// class rule
        //public virtual FormRepeaterTypeBuilder<TMainEntity, TEntity> Rule([NotNullAttribute] Type ruleType, FormRuleTriggers trigger = FormRuleTriggers.Changed)
        //{
        //    _repeaterField.Rules.Add(new FieldRule { RuleType = ruleType, Trigger = trigger });
        //    return this;
        //}

        //public virtual void InlineButton(string text, string hint = null)
        //{
        //    var bindingProperty = text;

        //    _fields[bindingProperty] = new DataField { Button = true, BindingProperty = bindingProperty, Label = hint, 
        //        BindingType = FieldBindingType.ActionButton, Group = Group };
        // }

        //// ToDo: add support of named format parameters like actionLink: "Form2/{id}/{id2}", where id, id2 - named page parameters
        //public virtual FormRepeaterTypeBuilder<TMainEntity, TEntity> DialogButton(ButtonActionTypes actionType, string text = null, string hint = null, string actionLink = null)
        //{
        //    ControlButtons.Add(new DialogButtonDetails { Action = actionType, Text = text, Hint = hint, LinkText = actionLink });
        //    return this;
        //}

        //public virtual FormRepeaterTypeBuilder<TMainEntity,TEntity> DialogButton(string actionLink, ButtonActionTypes actionType, string text = null, string hint = null)
        //{
        //    ControlButtons.Add(new DialogButtonDetails { Action = actionType, Text = text, Hint = hint, LinkText = actionLink });
        //    return this;
        //}
    }
}
