using BlazorForms.Forms.Definitions.FluentForms.Model;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using BlazorForms.Forms.Definitions.FluentForms.Rules;

namespace BlazorForms.Forms
{
    public abstract class FormEditBase<TModel> : IModelDefinitionForm
        where TModel : class
    {
        protected readonly IDataFieldProcessor _dataFieldProcessor;
        protected FormEntityTypeBuilder<TModel> _builder;

        private Dictionary<Type, string> _formats = new Dictionary<Type, string>()
        {
            { typeof(DateTime), "dd/MM/yyyy" }
            ,{ typeof(DateTime?), "dd/MM/yyyy" }
        };

        public FormEditBase()
        {
            _dataFieldProcessor = new DefaultDataFieldProcessor();
            _builder = new FormEntityTypeBuilder<TModel>();
            Define(_builder);
            _builder.AssertValid();

            DisplayName = _builder.DisplayName;
            Layout = _builder.Layout;
            ChildProcess = _builder.ChildProcess;
            Access = _builder.Access;
        }

        public string DisplayName { get; private set; }
        public FormLayout Layout { get; private set; }
        public Type ChildProcess { get; private set; }
        public FormAllowAccess Access { get; private set; }
        public string ItemsPath { get; private set; }

        IEnumerable<DataField> IModelDefinitionForm.GetDetailsFields()
        {
            var fields = new List<DataField>();
            fields.AddRange(_builder.Fields.OrderBy(f => f.Order));
            var repeaterFields = _builder.RepeaterBuilders.SelectMany(r => r.Fields.OrderBy(f => f.Order));
            fields.AddRange(repeaterFields);

            if (fields.Any())
            {
                _dataFieldProcessor.PrepareFields(fields.ToList(), GetDetailsType());
            }

            return fields;
        }

        public Type GetDetailsType()
        {
            return typeof(TModel);
        }

        public IEnumerable<DialogButtonDetails> GetButtons()
        {
            var result = _builder.ActionButtons; 
            return result;
        }

        public IEnumerable<ConfirmationDetails> GetConfirmations()
        {
            var result = _builder.Confirmations;
            return result;
        }

        public IEnumerable<IBindingFlowReference> GetButtonNavigations()
        {
            var result = new List<IBindingFlowReference>();
            return result;
        }

        // ToDo: should it be moved to DataEntryProvider?
        public string GetFieldFormat(DataField field)
        {
            var format = field.Format ?? FindDefaultFormat(field.DataType);
            return format;
        }

        protected abstract void Define(FormEntityTypeBuilder<TModel> builder);

        public virtual IFormRule<TModel> RootRule()
        {
            return null;
        }

        private string FindDefaultFormat(Type dataType)
        {
            if (_formats.ContainsKey(dataType))
            {
                return _formats[dataType];
            }

            return "";
        }

        public IEnumerable<ActionRouteLink> GetContextLinks()
        {
            return new List<ActionRouteLink>();
        }
    }
}
