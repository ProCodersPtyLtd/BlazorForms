using BlazorForms.Forms.Definitions.FluentForms.Model;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Forms
{
    public abstract class FormListBase<TModel> : IModelDefinitionForm
        where TModel : class
    {
        protected readonly IDataFieldProcessor _dataFieldProcessor;
        protected FormListBuilder<TModel> _builder;

        private Dictionary<Type, string> _formats = new Dictionary<Type, string>()
        {
            { typeof(DateTime), "dd/MM/yyyy" }
            ,{ typeof(DateTime?), "dd/MM/yyyy" }
        };

        public FormListBase()
        {
            _dataFieldProcessor = new DefaultDataFieldProcessor();
            _builder = new FormListBuilder<TModel>();
            Define(_builder);
            _builder.Builder.AssertValid();

            ItemsPath = _builder.Builder.ItemsPath;
            DisplayName = _builder.Builder.DisplayName;
            ChildProcess = _builder.Builder.ChildProcess;
            Access = _builder.Builder.Access;
        }

        public string DisplayName { get; private set; }
        public Type ChildProcess { get; private set; }
        public FormAllowAccess Access { get; private set; }
        public string ItemsPath { get; private set; }

        IEnumerable<DataField> IModelDefinitionForm.GetDetailsFields()
        {
            var fields = new List<DataField>();
            fields.AddRange(_builder.Builder.Fields.OrderBy(f => f.Order));

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

        public IEnumerable<ActionRouteLink> GetContextLinks()
        {
            var result = _builder.Builder.ContextLinks;
            return result;
        }

        public IEnumerable<DialogButtonDetails> GetButtons()
        {
            var result = new List<DialogButtonDetails>();
            return result;
        }

        public IEnumerable<IBindingFlowReference> GetButtonNavigations()
        {
            var result = _builder.Builder.RefButtons;
            return result;
        }
         
        // ToDo: should it be moved to DataEntryProvider?
        public string GetFieldFormat(DataField field)
        {
            var format = field.Format ?? FindDefaultFormat(field.DataType);
            return format;
        }

        protected abstract void Define(FormListBuilder<TModel> builder);

        private string FindDefaultFormat(Type dataType)
        {
            if (_formats.ContainsKey(dataType))
            {
                return _formats[dataType];
            }

            return "";
        }

        public IEnumerable<ConfirmationDetails> GetConfirmations()
        {
            return new List<ConfirmationDetails>();
        }
    }
}
