using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform
{
    public class SelectableListItem
    {
        public int Id { get; set; }
        public string PrimaryText { get; set; }
        public string SecondaryText { get; set; }
        public object Object { get; set; }
    }
}
