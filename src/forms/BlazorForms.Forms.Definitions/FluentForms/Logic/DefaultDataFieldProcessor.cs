using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Forms
{
    public class DefaultDataFieldProcessor : IDataFieldProcessor
    {
        public List<DataField> PrepareFields(List<DataField> list, Type type)
        {
            foreach (var field in list)
            {
                SetDefaultName(field);
                ResolveControlType(type, field);
                ResolveLabel(type, field);
                SetUpBinding(type, field);
            }

            return list;
        }

        public void SetUpBinding(Type type, DataField f)
        {
            var fieldBinding = new FieldBinding
            {
                Binding = f.BindingProperty,
                TableBinding = f.TableBindingProperty,
                //BindingControlType = typeof(BindingControlType).Name,
                BindingControlType = f.BindingControlType ?? typeof(BindingControlType).Name,
                BindingType = f.BindingType,
                //dropdown
                ItemsBinding = f.SelectItemsProperty,
                IdBinding = f.SelectIdProperty,
                NameBinding = f.SelectNameProperty
            };

            // Set FastReflection delegates here?

            f.Binding = fieldBinding;
        }

        public void ResolveLabel(Type type, DataField field)
        {
            if (field.Label == null)
            {
                field.Label = field.Name;
            }
        }

        public void SetDefaultName(DataField f)
        {
            f.Name = f.Name ?? (f.BindingProperty ?? f.TableBindingProperty).Replace("$", "").Replace(".", "");
        }

        public void ResolveControlType(Type type, DataField field)
        {
            //if (field.Button || field.BindingControlType == typeof(TableColumnBindingControlType))
            if (field.BindingType == FieldBindingType.TableColumn)
            {
                if (string.IsNullOrWhiteSpace(field.ControlTypeName))
                {
                    field.ControlType = field.ControlType ?? typeof(TextEdit);
                }
            }

            if (field.Button || field.BindingType != FieldBindingType.SingleField)
            {
                return;
            }

            var property = type.GetPropertyByJsonPath(field.BindingProperty);
            field.DataType = property.PropertyType;
            var dataTypeName = field.DataType.Name;

            if (field.DataType.Name == "Nullable`1")
            {
                dataTypeName = Nullable.GetUnderlyingType(field.DataType).Name;
            }

            switch (dataTypeName)
            {
                case "Int32":
                case "Float":
                case "Double":
                    field.ViewModeControlType = field.ViewModeControlType ?? typeof(DefaultFormattedViewControl); // not gonna use it
                    break;
                case "Decimal":
                    field.ViewModeControlType = field.ViewModeControlType ?? typeof(DefaultFormattedViewControl); // not gonna use it
                    break;
                case "Boolean":
                    field.ViewModeControlType = field.ViewModeControlType ?? typeof(DefaultReadonlyCheckboxControl); // not gonna use it
                    break;
                case "DateTime":
                    field.ViewModeControlType = field.ViewModeControlType ?? typeof(DefaultFormattedViewControl); // not gonna use it
                    break;
                default:
                    field.ViewModeControlType = field.ViewModeControlType ?? typeof(DefaultFormattedViewControl); // not gonna use it
                    break;
            }

            if (!string.IsNullOrEmpty(field.ControlTypeName))
            {
                // if control type name specified we don't neet to override
                return;
            }

            switch (dataTypeName)
            {
                case "Int32":
                case "Float":
                case "Double":
                    field.ControlType = field.ControlType ?? typeof(TextEdit);
                    break;
                case "Decimal":
                    field.ControlType = field.ControlType ?? typeof(MoneyEdit);
                    break;
                case "Boolean":
                    field.ControlType = field.ControlType ?? typeof(Checkbox);
                    break;
                case "DateTime":
                    field.ControlType = field.ControlType ?? typeof(DateEdit);
                    break;
                default:
                    field.ControlType = field.ControlType ?? typeof(TextEdit);
                    break;
            }
        }
    }
}
