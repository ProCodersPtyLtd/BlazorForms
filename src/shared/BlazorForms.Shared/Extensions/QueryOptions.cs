using System;
using System.Collections.Generic;
using System.Text;
using BlazorForms.Shared.Extensions;

namespace BlazorForms.Shared.Extensions
{
    public class QueryOptions
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int PageReturnTotalCount { get; set; } = -1;
        public string SearchString { get; set; }
        public SortDirection SortDirection { get; set; }
        public string SortColumn { get; set; }
        public bool AllowSort { get; set; } = false;
        public bool AllowFiltering { get; set; } = false;
        public bool AllowPagination { get; set; } = false;
        public List<FieldFilter> Filters { get; set; } = new List<FieldFilter>();
        public Dictionary<string, string> FieldMappings { get; set; }

        public int Skip
        {
            get
            {
                return PageIndex * PageSize;
            }
        }

        public int Take
        {
            get
            {
                return PageSize;
            }
        }

        public string GetFieldMapping(string field)
        {
            if (FieldMappings != null && FieldMappings.ContainsKey(field))
            {
                return FieldMappings[field];
            }

            return field;
        }

        public class FieldFilter
        {
            public string BindingProperty { get; set; }
            public string Filter { get; set; }
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
}
