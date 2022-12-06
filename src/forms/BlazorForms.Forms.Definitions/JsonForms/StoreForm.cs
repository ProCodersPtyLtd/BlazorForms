using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Forms
{
    public class StoreForm : IStoreObject
    {
        // new properties
        public string DisplayName { get; set; }
        public string ChildProcessName { get; set; }

        public string Name { get; set; }
        public string Namespace { get; set; }

        // EF DbContext or DynamicContext is used, DynamicContext is generated from StoreSchema
        public bool IsDynamicContext { get; set; }
        public bool IsListForm { get; set; }
        public string Schema { get; set; }
        public string Datasource { get; set; }

        public List<StoreFormField> Fields { get; set; } = new List<StoreFormField>();
        public List<StoreFormButton> ActionButtons { get; set; } = new List<StoreFormButton>();

        // Page Properties
        public string Caption { get; set; }
        public string RoutingPath { get; set; }
        public List<StorePageParameter> PageParameters { get; set; } = new List<StorePageParameter> ();
        public string PageHeaderForm { get; set; }
        public bool PageHeaderFormReadOnly { get; set; }

        public bool Validated { get; set; }

        //public string GetRoutingPath()
        //{
        //    if (!string.IsNullOrWhiteSpace(RoutingPath))
        //    {
        //        return RoutingPath;
        //    }

        //    return Name;
        //}
    }

    public class StorePageParameter
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public int Order { get; set; }

        // Mapping to Query StoreQueryParameter.Name of Datasource, can be null if not mapped
        public string DatasourceQueryParameterMapping { get; set; }

        // Mapping to parameter in HeaderForm
        public string HeaderFormParameterMapping { get; set; }
    }

    public class StoreFormField
    {
        // new properties
        public string Name { get; set; }
        public bool Highlighted { get; set; }
        public bool Password { get; set; }
        public string Hint { get; set; }
        public bool NoCaption { get; set; }
        public string Group { get; set; }

        public string BindingProperty { get; set; }
        public string TableBindingProperty { get; set; }
        public string BindingControlType { get; set; }

        public string DataType { get; set; }
        public string ControlType { get; set; }
        // ControlReadOnly
        public string ViewModeControlType { get; set; }
        public string Label { get; set; }
        public bool Required { get; set; }
        public bool Hidden { get; set; }
        public bool? ReadOnly { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Unique { get; set; }
        public string Format { get; set; }
        public int Order { get; set; }
        public bool Filter { get; set; }
        public string FilterType { get; set; }
        public List<StoreFieldRule> Rules { get; set; }
    }

    public class StoreFormButton
    {
        public string Action { get; set; }
        public string Text { get; set; }
        public string ControlType { get; set; }
        public string Hint { get; set; }
        public int Order { get; set; }
        public bool Hidden { get; set; }
        public bool? ReadOnly { get; set; }
        //public string LinkText { get; set; }
        public string NavigationTargetForm { get; set; }
        public List<StoreNavigationParameter> NavigationParameterMapping { get; set; } = new List<StoreNavigationParameter>();
    }

    public class StoreNavigationParameter
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public int Order { get; set; }
        public string SupplyingParameterMapping { get; set; }

    }

    public class StoreFieldRule
    {
        public string Name { get; set; }
        public string Trigger { get; set; }
        public string Code { get; set; }
    }
}
