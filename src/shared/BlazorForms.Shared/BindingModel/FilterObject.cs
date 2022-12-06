using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared
{
    public class FilterObject
    {
        public string FilterValue { get; set; }
        public FieldFilterType FilterType { get; set; }
        public string? Date { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? Number { get; set; }
        public string? GreaterThan { get; set; }
        public string? LessThan { get; set; }
        public string? Text { get; set; }

    }
}
