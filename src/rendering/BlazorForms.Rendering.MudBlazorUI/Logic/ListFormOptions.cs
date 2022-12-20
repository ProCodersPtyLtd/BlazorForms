using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.MudBlazorUI.Components
{
    public class ListFormOptions : FormOptions
    {
        public DataOptimization DataOptimization { get; set; }
        public bool Virtualize { get; set; }
        public bool ShowSearch { get; set; }
        public bool ServerSideDataOptimization { get; set; }
        public string? ToolBarCaption { get; set; }
        public bool ShowFilters { get; set; }
        public bool ShowSorting { get; set; }
        public bool ShowPagination { get; set; }
        public int PageSize { get; set; } = 20;
        public int[]? PageSizeOptions { get; set; } = new int[] { 20, 50, 100 };
    }

    public enum DataOptimization
    {
        Virtualization,
        VirtualPagination,
        ServerPagination,
        None,
    }
}
