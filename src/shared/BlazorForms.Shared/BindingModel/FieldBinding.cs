using BlazorForms.Shared.BindingModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared
{
    public interface IFieldBinding
    {

    }

    public interface IFastReflectionBinding
    {
        public Func<object, object> FastReflectionGetter { get; }  
        public Action<object, object> FastReflectionSetter { get; }  
    }

    public class FieldBinding : IFieldBinding, IFastReflectionBinding
    {
        // IFastReflectionBinding
        [JsonIgnore]
        public Func<object, object> FastReflectionGetter { get; set; }

        [JsonIgnore]
        public Func<object, object> FastReflectionNameGetter { get; set; }

        [JsonIgnore]
        public Func<object, object> FastReflectionIdGetter { get; set; }

        [JsonIgnore]
        public Func<object, object> FastReflectionItemsGetter { get; set; }

        [JsonIgnore]
        public Func<object, object> FastReflectionTableGetter { get; set; }

        [JsonIgnore]
        public Action<object, object> FastReflectionSetter { get; set; }

        // IFieldBinding
        public const string ColumnIndexMarker = "[__index]";
        public string Binding { get; set; }
        public string ItemsBinding { get; set; }
        public string IdBinding { get; set; }
        public string NameBinding { get; set; }
        public string TableBinding { get; set; }
        public string TargetBinding { get; set; }
        public int? RowIndex { get; set; }
        public BindingParameters Parameters { get; set; }
        public FieldBindingType BindingType { get; set; }
        public string BindingControlType { get; set; }
        public ActionType ActionType { get; set; }
        public List<BindingFlowAction> ContextMenuActions { get; set; }
        public string FilterRefField { get; set; }

        public string FilterType { get; set; }
        public FieldBinding CopyWithKey()
        {
            var b = new FieldBinding
            {
                Binding = Binding,
                ItemsBinding = ItemsBinding,
                IdBinding = IdBinding,
                NameBinding = NameBinding,
                TableBinding = TableBinding,
                TargetBinding = TargetBinding ,
                BindingType = BindingType,
                BindingControlType = BindingControlType,
                RowIndex = RowIndex,
                FilterType = FilterType,
                FilterRefField = FilterRefField
            };

            return b;
        }

        public FieldBindingPathType GetPathType()
        {
            if (BindingType == FieldBindingType.Form || Binding == null || 
                BindingType == FieldBindingType.Repeater || BindingType == FieldBindingType.Table)
            {
                return FieldBindingPathType.Unsupported;
            }
            else if (BindingType == FieldBindingType.SingleSelect)
            {
                return FieldBindingPathType.SingleSelect;
            }
            else if (BindingType == FieldBindingType.TableColumn || BindingType == FieldBindingType.TableColumnSingleSelect)
            {
                return FieldBindingPathType.Column;
            }

            return FieldBindingPathType.Straight;
        }

        //public bool IsStraight()
        //{
        //    return BindingType != FieldBindingType.Form && Binding != null
        //        // not implemented yet
        //        // TableColumn binding refers to Item - not to Model 
        //        //&& BindingType != FieldBindingType.TableColumn && BindingType != FieldBindingType.TableColumnSingleSelect
        //        ;
        //}

        public bool IsResolved
        {
            get
            {
                var result = !Key?.Contains(ColumnIndexMarker);
                return result == true;
            }
        }

        /// <summary>
        /// Return unique key for a field inside Task Definition
        /// like: $.Members[__index].Login
        /// </summary>
        public string TemplateKey
        {
            get
            {
                if(!string.IsNullOrWhiteSpace(TableBinding))
                {
                    return $"{TableBinding}{ColumnIndexMarker}{Binding?.Replace("$", "")}";
                }

                return Binding;
            }
        }

        /// <summary>
        /// Returns unique key for a physical field inside Task
        /// like: $.Members[1].Login
        /// </summary>
        public string Key
        {
            get
            {
                if(RowIndex != null)
                {
                    return GetResolvedKey(RowIndex.Value);
                }

                return TemplateKey;
            }
        }

        public string ResolvedBinding
        {
            get
            {
                if (BindingType == FieldBindingType.SelectableList)
                {
                    return TargetBinding;
                }

                return Key;
            }
        }

        public void ResolveKey(FieldBindingArgs args)
        {
            RowIndex = args.RowIndex;
        }

        public string GetResolvedKey(int rowIndex)
        {
            var result = TemplateKey;
            result = result.Replace(ColumnIndexMarker, $"[{rowIndex}]");
            return result;
        }
    }

    public class FieldBindingArgs
    {
        public int RowIndex { get; set; }
    }
}
