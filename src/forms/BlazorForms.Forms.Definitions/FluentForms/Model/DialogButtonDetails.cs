using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Forms
{
    public class DialogButtonDetails
    {
        public ButtonActionTypes Action { get; set; }
        public string Text { get; set; }
        public string Binding { get; set; }
        public string Hint { get; set; }
        public string LinkText { get; set; }
        public int Order { get; set; }
    }
}
