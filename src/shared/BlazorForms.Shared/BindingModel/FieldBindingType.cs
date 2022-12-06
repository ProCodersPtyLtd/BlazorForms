using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Shared
{
    public enum FieldBindingType
    {
        SingleField = 1,
        SingleSelect,
        SelectableList,
        Table,
        Repeater,
        TableColumn,
        TableColumnSingleSelect,
        TableCount,
        TableFooter,
        TableColumnContextMenu,
        ActionButton,
        ListFormContextMenu,
        FlowReferenceButtons,
        ListFormContextMenuItem,
        FlowReferenceButtonsItem,
        Form,
    }

    public enum FieldBindingPathType
    {
        Unsupported,
        Straight,
        Column,
        SingleSelect
    }

    public enum ActionType
    {
        Custom = 1,
        Cancel,
        Close,
        Submit, //approve
        Save,
        Validate,
        Ignore,
        Reject,
        CloseFinish,
        SubmitClose
    }

}
