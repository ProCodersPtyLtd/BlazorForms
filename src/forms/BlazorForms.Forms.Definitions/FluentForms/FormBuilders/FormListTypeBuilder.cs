using BlazorForms.Forms;
using BlazorForms.Forms.Definitions.FluentForms.FormBuilders;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BlazorForms.Forms
{
    public abstract class FormListTypeBuilder : FormBuilderBase
    {
        protected FieldColumnBuilder _FieldColumnBuilder;
        protected readonly List<ActionRouteLink> _contextLinks = new List<ActionRouteLink>();
        public string ItemsPath { get; protected set; }
        public FieldColumnBuilder FieldColumnBuilder { get { return _FieldColumnBuilder; } }
        public IEnumerable<ActionRouteLink> ContextLinks { get { return _contextLinks; } }
        public List<IBindingFlowReference> RefButtons { get; private set; } = new List<IBindingFlowReference>();
        public abstract void AssertValid();
     }

    public class FormListTypeBuilder<TEntity> : FormListTypeBuilder where TEntity : class
    {
        protected int _propertyOrder;

        public FormListTypeBuilder(Expression items)
        {
            ItemsPath = items.ToString().ReplaceLambdaVar();
            // order of properties in the Entity has a low priority - all mentioned properties will be moved up 
            _propertyOrder = 0;
        }

        public virtual FieldColumnBuilder<TProperty, TEntity> Property<TProperty>([NotNullAttribute] Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            _propertyOrder++;
            var bindingProperty = propertyExpression.Body.ToString().ReplaceLambdaVar();
            CreateFieldIfNotExists(typeof(TProperty), bindingProperty);
            var resultField = _fields[bindingProperty];
            resultField.Order = _propertyOrder;

            // Set binding types
            resultField.TableBindingProperty = ItemsPath;
            resultField.BindingType = FieldBindingType.TableColumn;
            resultField.BindingControlType = typeof(TableColumnBindingControlType).Name;

            // explicitly mentioned property is not hidden anymore
            resultField.Hidden = false;
            var result = new FieldColumnBuilder<TProperty, TEntity>(resultField);
            _FieldColumnBuilder = result;

            return result;
        }

        private void CreateFieldIfNotExists(Type propertyType, string bindingProperty)
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

        /// <summary>
        /// Sets navigation context button
        /// </summary>
        /// <param name="text"></param>
        /// <param name="actionLink"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public virtual FormListTypeBuilder<TEntity> ContextButton(string text, string actionLink, 
            FlowReferenceOperation operation = FlowReferenceOperation.Unknown)
        {
            _contextLinks.Add(new ActionRouteLink { Text = text, LinkText = actionLink, IsNavigation = true, Operation = operation });
            return this;
        }

        /// <summary>
        /// Sets context button flow reference
        /// </summary>
        /// <param name="text"></param>
        /// <param name="flowType"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public virtual FormListTypeBuilder<TEntity> ContextButton(string text, Type flowType,
            FlowReferenceOperation operation = FlowReferenceOperation.Unknown)
        {
            _contextLinks.Add(new ActionRouteLink { Text = text, FlowType = flowType, IsNavigation = false, Operation = operation });
            return this;
        }

        public virtual FormListTypeBuilder<TEntity> NavigationButton(string name, Type flowType, FlowReferenceOperation? operation = null)
        {
            var refButton = new BindingFlowReference(name, flowType, operation);
            RefButtons.Add(refButton);
            return this;
        }

        public virtual FormListTypeBuilder<TEntity> NavigationButton(string name, string navigationFormat, FlowReferenceOperation? operation = null)
        {
            var refButton = new BindingFlowNavigationReference(name, navigationFormat, operation);
            RefButtons.Add(refButton);
            return this;
        }

        public virtual FormListTypeBuilder<TEntity> Rule([NotNullAttribute] Type ruleType, FormRuleTriggers trigger = FormRuleTriggers.Loaded)
        {
            _formField.Rules.Add(new FieldRule { RuleType = ruleType, Trigger = trigger });
            return this;
        }

        public override void AssertValid()
        {
            AssertValid<TEntity>();
        }
    }
}
