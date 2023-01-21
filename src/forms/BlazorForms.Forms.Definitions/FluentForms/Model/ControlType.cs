using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Forms
{
    public enum ControlType
    {
        Autocomplete = 1,
        Button,
        Checkbox,
        CustomComponent,
        DateEdit,
        DatePicker,
        DropDown,
        FileUpload,
        Header = 9,
        Label = 10,
        MoneyEdit,
        PercentEdit,
        Repeater,
        SelectableList,
        Subtitle,
        Table,
        TextArea,
        TextEdit,
        TextSearchEdit,
        ActionMenuItem,
        Form = 21,
        // for Quiz
        Image,
        Paragraph,
        Card,
        CardAvatar,
        CardTitle,
        CardBody,
    }
}
